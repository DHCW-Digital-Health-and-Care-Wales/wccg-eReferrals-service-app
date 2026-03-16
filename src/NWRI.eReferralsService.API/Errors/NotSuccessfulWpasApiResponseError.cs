using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;

namespace NWRI.eReferralsService.API.Errors;

public class NotSuccessfulWpasApiResponseError : BaseFhirHttpError
{
    public override string Code => FhirHttpErrorCodes.ReceiverServerError;
    public override string DiagnosticsMessage => "WPAS API returned an unsuccessful response.";
    public override OperationOutcome.IssueType IssueType => OperationOutcome.IssueType.Exception;
}
