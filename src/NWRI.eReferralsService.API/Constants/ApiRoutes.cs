using System.Diagnostics.CodeAnalysis;

namespace NWRI.eReferralsService.API.Constants;

[ExcludeFromCodeCoverage]
public static class ApiRoutes
{
    public const string HealthCheck = "/health";
    public const string HealthCheckLive = "/health/live";
    public const string HealthCheckReady = "/health/ready";
}
