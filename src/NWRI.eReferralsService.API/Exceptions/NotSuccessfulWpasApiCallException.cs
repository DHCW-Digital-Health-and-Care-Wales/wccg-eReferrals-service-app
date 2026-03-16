using System.Net;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Errors;

namespace NWRI.eReferralsService.API.Exceptions;

public class NotSuccessfulWpasApiCallException : BaseFhirException
{
    private string ExceptionMessage { get; }

    public HttpStatusCode StatusCode { get; init; }
    public override IEnumerable<BaseFhirHttpError> Errors { get; }
    public override string Message => ExceptionMessage;

    public NotSuccessfulWpasApiCallException(HttpStatusCode statusCode, string rawContent)
    {
        StatusCode = statusCode;
        Errors = [new NotSuccessfulWpasApiResponseError()];
        ExceptionMessage = $"API call returned: {(int)statusCode}. Raw content: {rawContent}";
    }
}
