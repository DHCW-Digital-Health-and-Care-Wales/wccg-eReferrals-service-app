using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NWRI.eReferralsService.API.Configuration;

[ExcludeFromCodeCoverage]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers, Reason = "Used by configuration binder")]
public class WpasApiConfig
{
    public static string SectionName => "WpasApi";

    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string CreateReferralEndpoint { get; init; }

    [Required]
    public required string CancelReferralEndpoint { get; init; }

    [Required]
    public required string GetReferralEndpoint { get; init; }

    [Required]
    public required int TimeoutSeconds { get; init; }
}
