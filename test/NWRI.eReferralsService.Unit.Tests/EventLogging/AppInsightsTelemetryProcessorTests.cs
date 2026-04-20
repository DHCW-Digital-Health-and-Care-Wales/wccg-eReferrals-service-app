using FluentAssertions;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.EventLogging;

namespace NWRI.eReferralsService.Unit.Tests.EventLogging;

public class AppInsightsTelemetryProcessorTests
{
    [Fact]
    public void ProcessShouldFilterExactHealthPath()
    {
        var spy = new TelemetryProcessorSpy();
        var sut = new AppInsightsTelemetryProcessor(spy);
        var telemetry = new RequestTelemetry { Url = new Uri($"https://localhost{ApiRoutes.HealthCheckPath}") };

        sut.Process(telemetry);

        spy.ProcessedTelemetry.Should().BeNull();
    }

    [Fact]
    public void ProcessShouldAllowHealthReadyPath()
    {
        var spy = new TelemetryProcessorSpy();
        var sut = new AppInsightsTelemetryProcessor(spy);
        var telemetry = new RequestTelemetry { Url = new Uri("https://localhost/health/ready") };

        sut.Process(telemetry);

        spy.ProcessedTelemetry.Should().BeSameAs(telemetry);
    }

    [Fact]
    public void ProcessShouldAllowHealthLivePath()
    {
        var spy = new TelemetryProcessorSpy();
        var sut = new AppInsightsTelemetryProcessor(spy);
        var telemetry = new RequestTelemetry { Url = new Uri("https://localhost/health/live") };

        sut.Process(telemetry);

        spy.ProcessedTelemetry.Should().BeSameAs(telemetry);
    }

    [Fact]
    public void ProcessShouldAllowNonHealthRequestTelemetry()
    {
        var spy = new TelemetryProcessorSpy();
        var sut = new AppInsightsTelemetryProcessor(spy);
        var telemetry = new RequestTelemetry { Url = new Uri("https://localhost/api/referrals") };

        sut.Process(telemetry);

        spy.ProcessedTelemetry.Should().BeSameAs(telemetry);
    }

    [Fact]
    public void ProcessShouldAllowNonRequestTelemetry()
    {
        var spy = new TelemetryProcessorSpy();
        var sut = new AppInsightsTelemetryProcessor(spy);
        var telemetry = new TraceTelemetry("test");

        sut.Process(telemetry);

        spy.ProcessedTelemetry.Should().BeSameAs(telemetry);
    }

    private sealed class TelemetryProcessorSpy : ITelemetryProcessor
    {
        public ITelemetry? ProcessedTelemetry { get; private set; }

        public void Process(ITelemetry telemetry)
        {
            ProcessedTelemetry = telemetry;
        }
    }
}
