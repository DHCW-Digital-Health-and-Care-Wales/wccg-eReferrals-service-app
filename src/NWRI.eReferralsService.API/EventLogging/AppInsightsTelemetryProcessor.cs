using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using NWRI.eReferralsService.API.Constants;

namespace NWRI.eReferralsService.API.EventLogging;

public class HealthCheckTelemetryFilter : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public HealthCheckTelemetryFilter(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry requestTelemetry && requestTelemetry.Url != null &&
            string.Equals(ApiRoutes.HealthCheck, requestTelemetry.Url.AbsolutePath, StringComparison.OrdinalIgnoreCase) &&
            requestTelemetry.ResponseCode == "200")
        {
            return;
        }

        _next.Process(item);
    }
}
