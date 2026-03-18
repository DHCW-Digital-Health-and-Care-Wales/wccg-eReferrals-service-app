using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;

namespace NWRI.eReferralsService.API.Errors;

public class ReceiverUnavailableError : BaseFhirHttpError
{
    public override string Code => FhirHttpErrorCodes.ReceiverUnavailable;
    public override string DiagnosticsMessage => "WPAS API is currently unavailable";
    public override OperationOutcome.IssueType IssueType => OperationOutcome.IssueType.Exception;
}
