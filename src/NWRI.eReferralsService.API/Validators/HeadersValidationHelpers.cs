using System.Text.RegularExpressions;

namespace NWRI.eReferralsService.API.Validators;

public partial class HeadersValidationHelpers
{
    [GeneratedRegex(@"([a-zA-Z0-9-]+\|?)+", RegexOptions.CultureInvariant)]
    private static partial Regex ValidUseCaseRegex();

    private const string AcceptTypePart = "application/fhir+json";
    private const string AcceptVersionPart = "version=1.2.0";

    public static bool BeValidGuid(string? value)
    {
        return Guid.TryParse(value, out _);
    }

    public static bool ContainValidUseCaseValues(string? value)
    {
        return value is not null && ValidUseCaseRegex().IsMatch(value);
    }

    public static bool BeValidAcceptValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var valueSpan = value.AsSpan();

        var separatorIndex = valueSpan.IndexOf(';');
        if (separatorIndex < 0 || valueSpan.Count(';') > 1)
        {
            return false;
        }

        var firstPart = valueSpan[..separatorIndex].Trim();
        var secondPart = valueSpan[(separatorIndex + 1)..].Trim();

        return
            (firstPart.Equals(AcceptTypePart.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
             secondPart.Equals(AcceptVersionPart.AsSpan(), StringComparison.OrdinalIgnoreCase)) ||
            (secondPart.Equals(AcceptTypePart.AsSpan(), StringComparison.OrdinalIgnoreCase) &&
             firstPart.Equals(AcceptVersionPart.AsSpan(), StringComparison.OrdinalIgnoreCase));
    }
}
