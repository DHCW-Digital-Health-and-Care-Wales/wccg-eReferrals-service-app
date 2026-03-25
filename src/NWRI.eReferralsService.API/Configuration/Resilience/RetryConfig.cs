using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace NWRI.eReferralsService.API.Configuration.Resilience;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers, Reason = "Used by configuration binder")]
public class RetryConfig
{
    [Required]
    public bool IsExponentialDelay { get; init; }

    [Required]
    [Range(0, 60)]
    public int DelaySeconds { get; init; }

    [Required]
    [Range(0, 10)]
    public int MaxRetries { get; init; }
}
