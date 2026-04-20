using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.EventLogging;

namespace NWRI.eReferralsService.Unit.Tests.EventLogging;

public class AppInsightsTelemetryProcessorTests
{
    [Fact]
    public void ProcessShouldFilterHealthCheckPath()
    {
        var next = new Mock<ITelemetryProcessor>();
        var sut = new AppInsightsTelemetryProcessor(next.Object);
        var telemetry = new RequestTelemetry { Url = new Uri($"https://localhost{ApiRoutes.HealthCheckPath}") };

        sut.Process(telemetry);

        next.Verify(x => x.Process(It.IsAny<ITelemetry>()), Times.Never);
    }

    [Theory]
    [InlineData("/health/ready")]
    [InlineData("/health/live")]
    [InlineData("/something/health")]
    [InlineData("/api/referrals")]
    public void ProcessShouldNotFilterRequestWhenPathIsNotExcluded(string path)
    {
        var next = new Mock<ITelemetryProcessor>();
        var sut = new AppInsightsTelemetryProcessor(next.Object);
        var telemetry = new RequestTelemetry { Url = new Uri($"https://localhost{path}") };

        sut.Process(telemetry);

        next.Verify(x => x.Process(telemetry), Times.Once);
    }
}
