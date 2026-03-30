using FluentValidation;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Helpers;
using NWRI.eReferralsService.API.Models;

namespace NWRI.eReferralsService.API.Validators;

public sealed class ReferralHeadersModelValidator : AbstractValidator<HeadersModel>
{
    public ReferralHeadersModelValidator(FhirBase64Decoder fhirBase64Decoder)
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        Include(new CommonHeadersModelValidator(fhirBase64Decoder));

        RuleFor(x => x.TargetIdentifier)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.TargetIdentifier))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(fhirBase64Decoder.CanDecode<Identifier>)
            .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.TargetIdentifier, nameof(Identifier)))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));

        RuleFor(x => x.UseContext)
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.UseContext))
            .WithErrorCode(nameof(ValidationErrorCode.MissingRequiredHeaderCode))
            .Must(HeadersValidationHelpers.ContainValidUseCaseValues)
            .WithMessage(ValidationMessages.NotExpectedFormat(
                RequestHeaderKeys.UseContext,
                RequestHeaderKeys.GetExampleValue(RequestHeaderKeys.UseContext)))
            .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));
    }
}
