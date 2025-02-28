using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Errors;

namespace WCCG.eReferralsService.API.Exceptions;

public class MissingRequiredHeaderException : BaseFhirException
{
    private readonly IEnumerable<string> _missingHeaders;

    public MissingRequiredHeaderException(IEnumerable<string> missingHeaders)
    {
        _missingHeaders = missingHeaders;
    }

    public override IEnumerable<BaseFhirHttpError> Errors => _missingHeaders.Select(h => new HeaderValidationError(h));
    public override OperationOutcome.IssueType IssueType => OperationOutcome.IssueType.Required;
    public override string Message => $"Missing required header(s): {string.Join(',', _missingHeaders)}";
}
