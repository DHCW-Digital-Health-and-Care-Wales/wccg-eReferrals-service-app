using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Errors;

public class UnexpectedError : BaseFhirHttpError
{
    private readonly Exception _exception;

    public UnexpectedError(Exception exception)
    {
        _exception = exception;
    }

    public override string Code => FhirHttpErrorCodes.ReceiverServerError;
    public override string DiagnosticsMessage => $"Unexpected error: {_exception.Message}";
}
