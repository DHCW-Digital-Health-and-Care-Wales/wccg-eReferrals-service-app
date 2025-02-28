using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Errors;

public class HeaderValidationError : BaseFhirHttpError
{
    private readonly string _headerName;

    public HeaderValidationError(string headerName)
    {
        _headerName = headerName;
    }

    public override string Code => FhirHttpErrorCodes.ReceiverBadRequest;
    public override string DiagnosticsMessage => $"Missing required header: {_headerName}";
}
