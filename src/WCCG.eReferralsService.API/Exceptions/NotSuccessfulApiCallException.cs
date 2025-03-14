using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;

namespace WCCG.eReferralsService.API.Exceptions;

public class NotSuccessfulApiCallException : BaseFhirException
{
    private const string ValidationErrorsKey = "validationErrors";

    public HttpStatusCode StatusCode { get; init; }

    private readonly Dictionary<HttpStatusCode, string> _fhirErrorCodeDictionary = new()
    {
        { HttpStatusCode.BadRequest, FhirHttpErrorCodes.ReceiverBadRequest },
        { HttpStatusCode.TooManyRequests, FhirHttpErrorCodes.TooManyRequests },
        { HttpStatusCode.InternalServerError, FhirHttpErrorCodes.ReceiverUnavailable }
        //todo: add NotFound for GetReferral
    };

    public NotSuccessfulApiCallException(HttpStatusCode statusCode, ProblemDetails problemDetails)
    {
        StatusCode = statusCode;
        Errors = GetErrors(problemDetails);
    }

    public override IEnumerable<BaseFhirHttpError> Errors { get; }
    public override string Message => $"API cal returned: {StatusCode}. {string.Join(';', Errors.Select(x => x.DiagnosticsMessage))}.";

    private IEnumerable<BaseFhirHttpError> GetErrors(ProblemDetails problemDetails)
    {
        if (problemDetails.Extensions.TryGetValue(ValidationErrorsKey, out var errorMessages))
        {
            var errorList = JsonSerializer.Deserialize<List<string>>(errorMessages!.ToString()!);
            return errorList!.Select(e => new NotSuccessfulApiResponseError(FhirHttpErrorCodes.ReceiverBadRequest, e));
        }

        if (problemDetails.Extensions.Count > 0)
        {
            var errorParts = problemDetails.Extensions.Select(pair => $"{pair.Key}: {JsonSerializer.Serialize(pair.Value)}");
            return
            [
                new NotSuccessfulApiResponseError(
                    _fhirErrorCodeDictionary.GetValueOrDefault(StatusCode, FhirHttpErrorCodes.ReceiverUnavailable),
                    string.Join(";", errorParts))
            ];
        }

        if (problemDetails.Detail is null)
        {
            return [new NotSuccessfulApiResponseError(FhirHttpErrorCodes.ReceiverUnavailable, "Unexpected error")];
        }

        return
        [
            new NotSuccessfulApiResponseError(
                _fhirErrorCodeDictionary.GetValueOrDefault(StatusCode, FhirHttpErrorCodes.ReceiverUnavailable), problemDetails.Detail)
        ];
    }
}
