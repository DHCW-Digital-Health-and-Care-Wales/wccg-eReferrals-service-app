using FluentValidation;

namespace NWRI.eReferralsService.API.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidHttpUrl<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(url =>
            string.IsNullOrWhiteSpace(url) ||
            (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
             (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        );
    }
}
