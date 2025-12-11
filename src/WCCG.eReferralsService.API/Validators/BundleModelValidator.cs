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

        RuleFor(x => x.MessageHeader)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(MessageHeader)));

        RuleFor(x => x.ServiceRequest)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(ServiceRequest)));

        RuleFor(x => x.Patient)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Patient)));

        RuleFor(x => x.Encounter)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(Encounter)));

        RuleFor(x => x.CarePlan)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(CarePlan)));

        RuleFor(x => x.HealthcareService)
            .NotNull()
            .WithMessage(ValidationMessages.MissingBundleEntity(nameof(HealthcareService)));

        RuleFor(x => x.Organizations)
            .Must(list => list.Count >= 2)
            .WithMessage(ValidationMessages.MinCardinality(nameof(Organization), 2));

        RuleFor(x => x.Practitioners)
            .Must(list => list.Count >= 1)
            .WithMessage(ValidationMessages.MinCardinality(nameof(Practitioner), 1));

        RuleFor(x => x.PractitionerRoles)
            .Must(list => list.Count >= 1)
            .WithMessage(ValidationMessages.MinCardinality(nameof(PractitionerRole), 1));

        RuleFor(x => x.Consents)
            .Must(list => list.Count >= 1)
            .WithMessage(ValidationMessages.MinCardinality(nameof(Consent), 1));
    }
}
