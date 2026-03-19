using System.Net;
using NWRI.eReferralsService.API.Errors;

namespace NWRI.eReferralsService.API.Exceptions;

public class NotSuccessfulWpasApiCallException : BaseFhirException
{
    public HttpStatusCode StatusCode { get; }
    public override IEnumerable<BaseFhirHttpError> Errors { get; }
    public override string Message { get; }

    public NotSuccessfulWpasApiCallException(HttpStatusCode statusCode, string rawContent)
    {
        if ((int)statusCode > 500)
        {
            StatusCode = HttpStatusCode.ServiceUnavailable;
            Errors = [new ReceiverUnavailableError()];
        }
        else
        {
            StatusCode = HttpStatusCode.InternalServerError;
            Errors = [new ReceiverServerError()];
        }
        Message = $"WPAS API call failed with status code: {(int)statusCode}. Raw content: {rawContent}";
    }
}
