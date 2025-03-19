using System.ComponentModel.DataAnnotations;

namespace WCCG.eReferralsService.API.Configuration.Resilience;

public class RateLimiterConfig
{
    [Required]
    [Range(0, int.MaxValue)]
    public required int QueueLimit { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public required int PermitLimit { get; set; }
}
