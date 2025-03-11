using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Extensions;

namespace WCCG.eReferralsService.API.Models;

public class BundleModel
{
    public required ServiceRequest? ServiceRequest { get; set; }
    public required Patient? Patient { get; set; }
    public required Encounter? Encounter { get; set; }
    public required Appointment? Appointment { get; set; }
    public required Practitioner? RequestingPractitioner { get; set; }
    public required Practitioner? ReceivingClinicianPractitioner { get; set; }
    public required Organization? DhaOrganization { get; set; }
    public required Organization? ReferringPracticeOrganization { get; set; }

    public static BundleModel FromBundle(Bundle bundle)
    {
        return new BundleModel
        {
            ServiceRequest = bundle.ResourceByType<ServiceRequest>(),
            Patient = bundle.ResourceByType<Patient>(),
            Encounter = bundle.ResourceByType<Encounter>(),
            Appointment = bundle.ResourceByType<Appointment>(),
            RequestingPractitioner = bundle.ResourceByType<Practitioner>(FhirConstants.RequestingPractitionerId),
            ReceivingClinicianPractitioner = bundle.ResourceByType<Practitioner>(FhirConstants.ReceivingClinicianId),
            DhaOrganization = bundle.ResourceByType<Organization>(FhirConstants.DhaCodeId),
            ReferringPracticeOrganization = bundle.ResourceByType<Organization>(FhirConstants.ReferringPracticeId),
        };
    }
}
