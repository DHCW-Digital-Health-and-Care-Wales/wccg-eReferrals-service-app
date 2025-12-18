using FluentValidation;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Models;
using static WCCG.eReferralsService.API.Constants.ValidationMessages;

namespace WCCG.eReferralsService.API.Validators;

public class BundleModelValidator : AbstractValidator<BundleModel>
{
    public BundleModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;

        RuleFor(x => x.MessageHeader)
            .NotNull()
            .WithMessage(MissingBundleEntity(nameof(MessageHeader)))
            .DependentRules(() =>
            {
                RuleFor(x => x.MessageHeader!.Definition)
                    .NotEmpty()
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Definition)));

                RuleFor(x => x.MessageHeader!.Meta)
                    .NotNull()
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Meta)));

                RuleFor(x => x.MessageHeader!.Destination)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Destination)));

                RuleFor(x => x.MessageHeader!.Sender)
                    .NotNull()
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Sender)));

                RuleFor(x => x.MessageHeader!.Source)
                    .NotNull()
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Source)));

                RuleFor(x => x.MessageHeader!.Event)
                    .NotNull()
                    .WithMessage(MissingEntityField<MessageHeader>("eventCoding"));

                RuleFor(x => x.MessageHeader!.Reason)
                    .NotNull()
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Reason)));

                RuleFor(x => x.MessageHeader!.Focus)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<MessageHeader>(nameof(MessageHeader.Focus)));
            });

        RuleFor(x => x.ServiceRequest)
            .NotNull()
            .WithMessage(MissingBundleEntity(nameof(ServiceRequest)))
            .DependentRules(() =>
            {
                RuleFor(x => x.ServiceRequest!.StatusElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Status)));

                RuleFor(x => x.ServiceRequest!.IntentElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Intent)));

                RuleFor(x => x.ServiceRequest!.Subject)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Subject)));

                RuleFor(x => x.ServiceRequest!.Encounter)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Encounter)));

                RuleFor(x => x.ServiceRequest!.AuthoredOnElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.AuthoredOn)));

                RuleFor(x => x.ServiceRequest!.BasedOn)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.BasedOn)));

                RuleFor(x => x.ServiceRequest!.Occurrence)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>("occurrencePeriod"));

                RuleFor(x => x.ServiceRequest!.Requester)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Requester)));

                RuleFor(x => x.ServiceRequest!.Performer)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Performer)));

                RuleFor(x => x.ServiceRequest!.Category)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Category)));

                RuleFor(x => x.ServiceRequest!.Meta)
                    .NotNull()
                    .WithMessage(MissingEntityField<ServiceRequest>(nameof(ServiceRequest.Meta)));
            });

        RuleFor(x => x.Encounter)
            .NotNull()
            .WithMessage(MissingBundleEntity(nameof(Encounter)))
            .DependentRules(() =>
            {
                RuleFor(x => x.Encounter!.Identifier)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<Encounter>(nameof(Encounter.Identifier)));

                RuleFor(x => x.Encounter!.StatusElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<Encounter>(nameof(Encounter.Status)));

                RuleFor(x => x.Encounter!.Class)
                    .NotNull()
                    .WithMessage(MissingEntityField<Encounter>(nameof(Encounter.Class)));

                RuleFor(x => x.Encounter!.Subject)
                    .NotNull()
                    .WithMessage(MissingEntityField<Encounter>(nameof(Encounter.Subject)));

                RuleFor(x => x.Encounter!.Period)
                    .NotNull()
                    .WithMessage(MissingEntityField<Encounter>(nameof(Encounter.Period)));

                RuleFor(x => x.Encounter!.Meta)
                    .NotNull()
                    .WithMessage(MissingEntityField<Encounter>(nameof(Encounter.Meta)));
            });

        RuleFor(x => x.CarePlan)
            .NotNull()
            .WithMessage(MissingBundleEntity(nameof(CarePlan)))
            .DependentRules(() =>
            {
                RuleFor(x => x.CarePlan!.StatusElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Status)));

                RuleFor(x => x.CarePlan!.IntentElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Intent)));

                RuleFor(x => x.CarePlan!.Activity)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Activity)));

                RuleFor(x => x.CarePlan!.Subject)
                    .NotNull()
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Subject)));

                RuleFor(x => x.CarePlan!.Encounter)
                    .NotNull()
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Encounter)));

                RuleFor(x => x.CarePlan!.Addresses)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Addresses)));

                RuleFor(x => x.CarePlan!.Meta)
                    .NotNull()
                    .WithMessage(MissingEntityField<CarePlan>(nameof(CarePlan.Meta)));
            });

        RuleFor(x => x.HealthcareService)
            .NotNull()
            .WithMessage(MissingBundleEntity(nameof(HealthcareService)))
            .DependentRules(() =>
            {
                RuleFor(x => x.HealthcareService!.ActiveElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<HealthcareService>(nameof(HealthcareService.Active)));

                RuleFor(x => x.HealthcareService!.Identifier)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<HealthcareService>(nameof(HealthcareService.Identifier)));

                RuleFor(x => x.HealthcareService!.Meta)
                    .NotNull()
                    .WithMessage(MissingEntityField<HealthcareService>(nameof(HealthcareService.Meta)));

                RuleFor(x => x.HealthcareService!.Name)
                    .NotEmpty()
                    .WithMessage(MissingEntityField<HealthcareService>(nameof(HealthcareService.Name)));

                RuleFor(x => x.HealthcareService!.ProvidedBy)
                    .NotNull()
                    .WithMessage(MissingEntityField<HealthcareService>(nameof(HealthcareService.ProvidedBy)));
            });

        RuleFor(x => x.Patient)
            .NotNull()
            .WithMessage(MissingBundleEntity(nameof(Patient)))
            .DependentRules(() =>
            {
                RuleFor(x => x.Patient!.Identifier)
                    .Must(ids => ids is { Count: > 0 })
                    .WithMessage(MissingEntityField<Patient>(nameof(Patient.Identifier)));

                RuleFor(x => x.Patient!.Name)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<Patient>(nameof(Patient.Name)));

                RuleFor(x => x.Patient!.BirthDate)
                    .NotEmpty()
                    .WithMessage(MissingEntityField<Patient>(nameof(Patient.BirthDate)));

                RuleFor(x => x.Patient!.GenderElement)
                    .NotNull()
                    .WithMessage(MissingEntityField<Patient>(nameof(Patient.Gender)));

                RuleFor(x => x.Patient!.Address)
                    .Must(list => list is { Count: > 0 })
                    .WithMessage(MissingEntityField<Patient>(nameof(Patient.Address)));
            });
    }
}
