namespace NWRI.eReferralsService.API.Constants;

public static class FhirConstants
{
    public const string FhirMediaType = "application/fhir+json";
    public const string HttpErrorCodesSystem = "https://fhir.nhs.uk/CodeSystem/http-error-codes";
    public const string OperationOutcomeProfile = "https://fhir.hl7.org.uk/StructureDefinition/UKCore-OperationOutcome";
    public const string BarsMessageReasonSystem = "https://fhir.nhs.uk/CodeSystem/message-reason-bars";
    public const string NhsNumberSystem = "https://fhir.nhs.uk/Id/nhs-number";
    public const string BarsMessageReasonNew = "new";
    public const string BarsMessageReasonUpdate = "update";
    public const string BarsServiceRequestCreateReferral = "BARSServiceRequest-request-referral";
    public const string BarsServiceRequestCancelReferral = "BARSServiceRequest-request-validation";
    public const string BarsLocationIncidentLocation = "BARSLocationIncidentLocation";
    public const string BarsFlagSceneSafety = "BARSFlagSceneSafety";
    public const string CreateReferralReceiverOrganisationName = "Receiving/performing Organization";
    public const string CreateReferralSenderOrganisationName = "Sender Organization";
    public const string CancelReferralReceiverOrganisationName = "Receiver Organization";
    public const string CancelReferralSenderOrganisationName = "Sender Organization";
}
