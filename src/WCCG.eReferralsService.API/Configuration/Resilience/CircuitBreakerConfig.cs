using System.ComponentModel.DataAnnotations;

namespace WCCG.eReferralsService.API.Configuration.Resilience;

public class CircuitBreakerConfig
{
    [Required]
    [Range(0, 100)]
    public int FailureRatioPercent { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int MinimumThroughput { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int SamplingDurationSeconds { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int BreakDurationSeconds { get; set; }
}
