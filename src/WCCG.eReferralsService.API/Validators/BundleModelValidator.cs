using FluentValidation;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Models;

namespace WCCG.eReferralsService.API.Validators;

public class BundleModelValidator : AbstractValidator<BundleModel>
{
    public BundleModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.ServiceRequest)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(ServiceRequest)));

        RuleFor(x => x.Patient)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Patient)));

        RuleFor(x => x.Encounter)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Encounter)));

        RuleFor(x => x.Appointment)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Appointment)));

        RuleFor(x => x.RequestingPractitioner)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Practitioner), FhirConstants.RequestingPractitionerId));

        RuleFor(x => x.ReceivingClinicianPractitioner)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Practitioner), FhirConstants.ReceivingClinicianId));

        RuleFor(x => x.DhaOrganization)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Organization), FhirConstants.DhaCodeId));

        RuleFor(x => x.ReferringPracticeOrganization)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Organization), FhirConstants.ReferringPracticeId));
    }
}
