namespace WCCG.eReferralsService.API.Constants;

public enum AuditEvents
{
    HeadersValidationSucceeded,
    HeadersValidationFailed,
    FhirProfileValidationSucceeded,
    FhirProfileValidationFailed,
    MandatoryDataValidationSucceeded,
    MandatoryDataValidationFailed,
}
