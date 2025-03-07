using System.Text;
using System.Text.Json;
using FluentValidation;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Models;

namespace WCCG.eReferralsService.API.Validators;

public class HeadersModelValidator : AbstractValidator<HeadersModel>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions().ForFhirExtended();

    private readonly List<string> _useCaseValues =
    [
        "a4t2",
        "validation",
        "servicerequest-response",
        "new"
    ];

    public HeadersModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Continue;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.TargetIdentifier)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.TargetIdentifier))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(BeValidFhirType<Identifier>)
            .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.TargetIdentifier, nameof(Identifier)))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);

        RuleFor(x => x.EndUserOrganisation)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.EndUserOrganisation))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(BeValidFhirType<Organization>)
            .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.EndUserOrganisation, nameof(Organization)))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);

        RuleFor(x => x.RequestingSoftware)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.RequestingSoftware))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(BeValidFhirType<Device>)
            .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.RequestingSoftware, nameof(Device)))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);

        When(x => !string.IsNullOrWhiteSpace(x.RequestingPractitioner), () =>
        {
            RuleFor(x => x.RequestingPractitioner!)
                //Format
                .Must(BeValidFhirType<PractitionerRole>)
                .WithMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.RequestingPractitioner, nameof(PractitionerRole)))
                .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);
        });

        RuleFor(x => x.RequestId)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.RequestId))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(BeValidGuid!)
            .WithMessage(ValidationMessages.NotGuidFormat(RequestHeaderKeys.RequestId))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);

        RuleFor(x => x.CorrelationId)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.CorrelationId))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(BeValidGuid!)
            .WithMessage(ValidationMessages.NotGuidFormat(RequestHeaderKeys.CorrelationId))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);

        RuleFor(x => x.UseContext)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.UseContext))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(ContainValidUseCaseValues!)
            .WithMessage(ValidationMessages.NotExpectedFormat(RequestHeaderKeys.UseContext,
                RequestHeaderKeys.GetExampleValue(RequestHeaderKeys.UseContext)))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);

        RuleFor(x => x.Accept)
            //Empty
            .NotEmpty()
            .WithMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.Accept))
            .WithErrorCode(ValidationErrorCodes.MissingRequiredHeaderCode)
            //Format
            .Must(HaveValidAcceptValue!)
            .WithMessage(ValidationMessages.NotExpectedFormat(RequestHeaderKeys.Accept,
                RequestHeaderKeys.GetExampleValue(RequestHeaderKeys.Accept)))
            .WithErrorCode(ValidationErrorCodes.InvalidHeaderCode);
    }

    private static bool BeValidGuid(string value)
    {
        return Guid.TryParse(value, out _);
    }

    private bool BeValidFhirType<T>(string? value) where T : Base
    {
        try
        {
            var bytes = Convert.FromBase64String(value!);
            var rawString = Encoding.UTF8.GetString(bytes);
            JsonSerializer.Deserialize<T>(rawString, _jsonSerializerOptions);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool ContainValidUseCaseValues(string value)
    {
        var parts = value.Split("|");
        return parts.Length > 0 && parts.All(_useCaseValues.Contains);
    }

    private bool HaveValidAcceptValue(string value)
    {
        var parts = value.Split(";", StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        if (!parts[0].Equals(FhirConstants.FhirMediaType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var versionParts = parts[1].Split('=', StringSplitOptions.TrimEntries);
        if (versionParts.Length != 2 || !versionParts[0].Equals("version", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var versionNumberParts = versionParts[1].Trim().Split('.', StringSplitOptions.TrimEntries);
        return versionNumberParts.Length > 0
               && versionNumberParts.All(x => int.TryParse(x, out _));
    }
}
