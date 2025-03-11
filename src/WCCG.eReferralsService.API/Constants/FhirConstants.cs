namespace WCCG.eReferralsService.API.Constants;

public static class FhirConstants
{
    public const string FhirMediaType = "application/fhir+json";
    public const string HttpErrorCodesSystem = "https://fhir.nhs.uk/CodeSystem/http-error-codes";
    public const string OperationOutcomeProfile = "https://fhir.hl7.org.uk/StructureDefinition/UKCore-OperationOutcome";
    public const string ReceivingClinicianId = "ReceivingClinician";
    public const string RequestingPractitionerId = "RequestingPractitioner";
    public const string DhaCodeId = "DhaCode";
    public const string ReferringPracticeId = "ReferringPractice";
}
