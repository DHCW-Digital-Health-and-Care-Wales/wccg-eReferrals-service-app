using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.EventLogging;

namespace NWRI.eReferralsService.Unit.Tests.EventLogging;

public class HealthCheckTelemetryFilterTests
{
    [Fact]
    public void ProcessShouldFilterHealthCheckPathOnlyWhenSuccessful()
    {
        var next = new Mock<ITelemetryProcessor>();
        var sut = new HealthCheckTelemetryFilter(next.Object);
        var telemetry = new RequestTelemetry 
        { 
            Url = new Uri($"https://localhost{ApiRoutes.HealthCheck}"),
            ResponseCode = "200"
        };

        sut.Process(telemetry);

        next.Verify(x => x.Process(It.IsAny<ITelemetry>()), Times.Never);
    }

    [Theory]
    [InlineData("500")]
    [InlineData("503")]
    [InlineData("404")]
    public void ProcessShouldNotFilterHealthCheckPathOnFailure(string statusCode)
    {
        var next = new Mock<ITelemetryProcessor>();
        var sut = new HealthCheckTelemetryFilter(next.Object);
        var telemetry = new RequestTelemetry 
        { 
            Url = new Uri($"https://localhost{ApiRoutes.HealthCheck}"),
            ResponseCode = statusCode
        };

        sut.Process(telemetry);

        next.Verify(x => x.Process(telemetry), Times.Once);
    }

    [Theory]
    [InlineData(ApiRoutes.HealthCheckReady)]
    [InlineData(ApiRoutes.HealthCheckLive)]
    [InlineData("/metadata")]
    public void ProcessShouldNotFilterRequestWhenPathIsNotExcluded(string path)
    {
        var next = new Mock<ITelemetryProcessor>();
        var sut = new HealthCheckTelemetryFilter(next.Object);
        var telemetry = new RequestTelemetry 
        { 
            Url = new Uri($"https://localhost{path}"),
            ResponseCode = "200"
        };

        sut.Process(telemetry);

        next.Verify(x => x.Process(telemetry), Times.Once);
    }

    [Fact]
    public void ProcessShouldForwardNonRequestTelemetry()
    {
        var next = new Mock<ITelemetryProcessor>();
        var sut = new HealthCheckTelemetryFilter(next.Object);
        var telemetry = new TraceTelemetry("test message");

        sut.Process(telemetry);

        next.Verify(x => x.Process(telemetry), Times.Once);
    }
}
