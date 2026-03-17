using System.Net;
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
        if ((int)statusCode is > 500 and < 600)
        {
            StatusCode = HttpStatusCode.ServiceUnavailable;
            Errors = [new ReceiverUnavailableError()];
        }
        else
        {
            StatusCode = HttpStatusCode.InternalServerError;
            Errors = [new ReceiverServerError()];
        }
        ExceptionMessage = $"WPAS API call failed with status code: {(int)statusCode}. Raw content: {rawContent}";
    }
}
