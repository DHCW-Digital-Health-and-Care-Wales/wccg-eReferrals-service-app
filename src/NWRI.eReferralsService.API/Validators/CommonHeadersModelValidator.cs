using FluentValidation;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Helpers;
using NWRI.eReferralsService.API.Models;

namespace NWRI.eReferralsService.API.Validators;

public sealed class CommonHeadersModelValidator : AbstractValidator<HeadersModel>
{
    public CommonHeadersModelValidator(FhirBase64Decoder fhirBase64Decoder)
    {
        RuleFor(x => x.EndUserOrganisation)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.EndUserOrganisation))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(fhirBase64Decoder.CanDecode<Organization>)
            .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.EndUserOrganisation, nameof(Organization)))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));

        RuleFor(x => x.RequestingSoftware)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.RequestingSoftware))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(fhirBase64Decoder.CanDecode<Device>)
            .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.RequestingSoftware, nameof(Device)))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));

        When(x => !string.IsNullOrWhiteSpace(x.RequestingPractitioner), () =>
        {
            RuleFor(x => x.RequestingPractitioner)
                .Must(fhirBase64Decoder.CanDecode<PractitionerRole>)
                .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.RequestingPractitioner, nameof(PractitionerRole)))
                .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));
        });

        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.RequestId))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(HeadersValidationHelpers.BeValidGuid)
            .WithMessage(ValidationMessages.NotGuidFormat(RequestHeaderKeys.RequestId))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));

        RuleFor(x => x.CorrelationId)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.CorrelationId))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(HeadersValidationHelpers.BeValidGuid)
            .WithMessage(ValidationMessages.NotGuidFormat(RequestHeaderKeys.CorrelationId))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));

        RuleFor(x => x.Accept)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.Accept))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(HeadersValidationHelpers.BeValidAcceptValue)
            .WithMessage(ValidationMessages.NotExpectedFormat(
                RequestHeaderKeys.Accept,
                RequestHeaderKeys.GetExampleValue(RequestHeaderKeys.Accept)))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));
    }
}
