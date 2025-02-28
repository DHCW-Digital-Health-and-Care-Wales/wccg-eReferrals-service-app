using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.API.Exceptions;

namespace WCCG.eReferralsService.API.Helpers;

public static class OperationOutcomeCreator
{
    public static OperationOutcome CreateOperationOutcome(OperationOutcome.IssueType issueType, params BaseFhirHttpError[] errors)
    {
        var issues = errors.Select(error => new OperationOutcome.IssueComponent
        {
            Severity = OperationOutcome.IssueSeverity.Error,
            Code = issueType,
            Details = new CodeableConcept(error.System, error.Code, error.Display),
            Diagnostics = error.DiagnosticsMessage
        }).ToList();

        return new OperationOutcome
        {
            Id = Guid.NewGuid().ToString(),
            Meta = new Meta { Profile = [FhirConstants.OperationOutcomeProfile] },
            Issue = issues
        };
    }

    public static OperationOutcome CreateOperationOutcome(BaseFhirException fhirException)
    {
        return CreateOperationOutcome(fhirException.IssueType, [.. fhirException.Errors]);
    }
}
