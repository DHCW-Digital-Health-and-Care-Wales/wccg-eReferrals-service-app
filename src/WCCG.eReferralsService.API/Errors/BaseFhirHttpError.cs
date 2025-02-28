using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Helpers;

namespace WCCG.eReferralsService.API.Errors;

public abstract class BaseFhirHttpError
{
    public string System => FhirConstants.HttpErrorCodesSystem;
    public string Display => FhirHttpErrorHelper.GetDisplayMessageByCode(Code);
    public abstract string Code { get; }
    public abstract string DiagnosticsMessage { get; }
}
