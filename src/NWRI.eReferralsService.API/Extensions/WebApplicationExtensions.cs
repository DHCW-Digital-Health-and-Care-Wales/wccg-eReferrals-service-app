using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NWRI.eReferralsService.API.Constants;

namespace NWRI.eReferralsService.API.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static void MapCustomHealthChecks(this WebApplication app)
    {
        // Readiness probe
        app.MapHealthChecks(ApiRoutes.HealthCheckReady, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready")
        });

        // Liveness probe
        app.MapHealthChecks(ApiRoutes.HealthCheckLive, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live")
        });

        // General health check endpoint
        app.MapHealthChecks(ApiRoutes.HealthCheckPath, new HealthCheckOptions());
    }
}
