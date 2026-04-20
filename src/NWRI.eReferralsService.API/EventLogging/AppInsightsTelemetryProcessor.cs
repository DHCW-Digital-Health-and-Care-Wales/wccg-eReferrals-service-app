using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using NWRI.eReferralsService.API.Constants;

namespace NWRI.eReferralsService.API.EventLogging;

public sealed class AppInsightsTelemetryProcessor : ITelemetryProcessor
{
    private static readonly string[] ExcludedFromTelemetryPaths = [ApiRoutes.HealthCheck];
    private readonly ITelemetryProcessor _next;

    public AppInsightsTelemetryProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry requestTelemetry && requestTelemetry.Url != null &&
            ExcludedFromTelemetryPaths.Contains(requestTelemetry.Url.AbsolutePath, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        _next.Process(item);
    }
}
