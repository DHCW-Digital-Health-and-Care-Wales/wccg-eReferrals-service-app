using FluentValidation;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Helpers;
using NWRI.eReferralsService.API.Models;

namespace NWRI.eReferralsService.API.Validators;

public sealed class MetadataHeadersModelValidator : AbstractValidator<HeadersModel>, IMetadataHeadersValidator
{
    public MetadataHeadersModelValidator(FhirBase64Decoder fhirBase64Decoder)
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        Include(new CommonHeadersModelValidator(fhirBase64Decoder));

        When(x => !string.IsNullOrWhiteSpace(x.TargetIdentifier), () =>
        {
            RuleFor(x => x.TargetIdentifier)
                .Must(fhirBase64Decoder.IsValid<Identifier>)
                .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.TargetIdentifier, nameof(Identifier)))
                .WithErrorCode(nameof(ValidationErrorCode.InvalidHeaderCode));
        });
    }
}
