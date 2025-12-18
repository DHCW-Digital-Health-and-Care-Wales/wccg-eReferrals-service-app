using Hl7.Fhir.Model;

namespace WCCG.eReferralsService.API.Validators;

public interface IFhirBundleProfileValidator
{
    OperationOutcome Validate(Bundle bundle);
}
