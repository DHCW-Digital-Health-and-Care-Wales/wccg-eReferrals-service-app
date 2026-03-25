using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace NWRI.eReferralsService.API.Configuration.Resilience;

[ExcludeFromCodeCoverage]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers, Reason = "Used by configuration binder")]
public class ResilienceConfig
{
    public static string SectionName => "Resilience";

    [ValidateObjectMembers]
    public required RetryConfig Retry { get; init; }

    [Required]
    [Range(0, 60)]
    public required int TotalTimeoutSeconds { get; init; }

    [Required]
    [Range(0, 60)]
    public required int AttemptTimeoutSeconds { get; init; }
}
