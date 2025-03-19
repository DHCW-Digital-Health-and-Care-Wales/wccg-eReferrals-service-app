using System.ComponentModel.DataAnnotations;

namespace WCCG.eReferralsService.API.Configuration.Resilience;

public class RetryConfig
{
    [Required]
    public bool IsExponentialDelay { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int DelaySeconds { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int MaxRetries { get; set; }
}
