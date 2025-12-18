using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Errors;

namespace WCCG.eReferralsService.API.Exceptions;

public class FhirProfileValidationException : BaseFhirException
{
    public FhirProfileValidationException(OperationOutcome operationOutcome)
    {
        OperationOutcome = operationOutcome ?? throw new ArgumentNullException(nameof(operationOutcome));
    }

    public OperationOutcome OperationOutcome { get; }

    public override IEnumerable<BaseFhirHttpError> Errors =>
        (OperationOutcome.Issue)
        .Select(i => new InvalidBundleError(
            i.Details?.Text
            ?? i.Diagnostics
            ?? i.Code?.ToString()
            ?? "FHIR profile validation failed."));

    public override string Message =>
        $"FHIR profile validation failure: {OperationOutcome.Issue?.Count ?? 0} issue(s).";
}
