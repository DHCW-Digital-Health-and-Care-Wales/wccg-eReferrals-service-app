using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Errors;

namespace WCCG.eReferralsService.API.Exceptions;

public abstract class BaseFhirException : Exception
{
    public abstract IEnumerable<BaseFhirHttpError> Errors { get; }
    public abstract OperationOutcome.IssueType IssueType { get; }
}
