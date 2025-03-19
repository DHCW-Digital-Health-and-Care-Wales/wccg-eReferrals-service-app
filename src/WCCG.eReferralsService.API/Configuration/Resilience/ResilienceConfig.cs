using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace WCCG.eReferralsService.API.Configuration.Resilience;

[ExcludeFromCodeCoverage]
public class ResilienceConfig
{
    public static string SectionName => "Resilience";

    [ValidateObjectMembers]
    public required RateLimiterConfig RateLimiter { get; set; }

    [ValidateObjectMembers]
    public required RetryConfig Retry { get; set; }

    [ValidateObjectMembers]
    public required CircuitBreakerConfig CircuitBreaker { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public required int TotalTimeoutSeconds { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public required int AttemptTimeoutSeconds { get; set; }
}
