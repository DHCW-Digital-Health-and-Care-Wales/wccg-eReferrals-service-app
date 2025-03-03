using System.Net;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Middleware;
using WCCG.eReferralsService.Unit.Tests.Extensions;
using Task = System.Threading.Tasks.Task;

namespace WCCG.eReferralsService.Integration.Tests.Middleware;

public class ResponseMiddlewareTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public async Task ShouldHandleMissingRequiredHeaderException()
    {
        //Arrange
        var exception = _fixture.Create<MissingRequiredHeaderException>();
        var requestId = _fixture.Create<string>();
        var correlationId = _fixture.Create<string>();

        var host = StartHostWithException(exception);

        //Act
        var response = await host.GetTestServer()
            .CreateRequest(HostProvider.TestEndpoint)
            .AddHeader(RequestHeaderKeys.RequestId, requestId)
            .AddHeader(RequestHeaderKeys.CorrelationId, correlationId)
            .GetAsync();

        //Assert
        response.Content.Headers.GetValues(HeaderNames.ContentType).Should()
            .NotBeNull()
            .And.Contain(FhirConstants.FhirMediaType);

        response.Headers.GetValues(RequestHeaderKeys.RequestId).Should()
            .NotBeNull()
            .And.Contain(requestId);

        response.Headers.GetValues(RequestHeaderKeys.CorrelationId).Should()
            .NotBeNull()
            .And.Contain(correlationId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var operationOutcome = JsonSerializer.Deserialize<OperationOutcome>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions().ForFhirExtended())!;
        operationOutcome.Issue.Should().AllSatisfy(component =>
        {
            component.Code.Should().Be(OperationOutcome.IssueType.Required);
            component.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            component.Diagnostics.Should().Contain("Missing required header");
        });
    }

    [Fact]
    public async Task ShouldHandleDefaultException()
    {
        //Arrange
        var exception = _fixture.Create<Exception>();
        var requestId = _fixture.Create<string>();
        var correlationId = _fixture.Create<string>();

        var host = StartHostWithException(exception);

        //Act
        var response = await host.GetTestServer()
            .CreateRequest(HostProvider.TestEndpoint)
            .AddHeader(RequestHeaderKeys.RequestId, requestId)
            .AddHeader(RequestHeaderKeys.CorrelationId, correlationId)
            .GetAsync();

        //Assert
        response.Content.Headers.GetValues(HeaderNames.ContentType).Should()
            .NotBeNull()
            .And.Contain(FhirConstants.FhirMediaType);

        response.Headers.GetValues(RequestHeaderKeys.RequestId).Should()
            .NotBeNull()
            .And.Contain(requestId);

        response.Headers.GetValues(RequestHeaderKeys.CorrelationId).Should()
            .NotBeNull()
            .And.Contain(correlationId);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var operationOutcome = JsonSerializer.Deserialize<OperationOutcome>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions().ForFhirExtended())!;
        operationOutcome.Issue.Should().AllSatisfy(component =>
        {
            component.Code.Should().Be(OperationOutcome.IssueType.Transient);
            component.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            component.Diagnostics.Should().Contain(exception.Message);
        });
    }

    [Fact]
    public async Task ShouldTryToAddHeadersWhenSuccessful()
    {
        //Arrange
        var requestId = _fixture.Create<string>();
        var correlationId = _fixture.Create<string>();

        var host = StartHost();

        //Act
        var response = await host.GetTestServer()
            .CreateRequest(HostProvider.TestEndpoint)
            .AddHeader(RequestHeaderKeys.RequestId, requestId)
            .AddHeader(RequestHeaderKeys.CorrelationId, correlationId)
            .GetAsync();

        //Assert
        response.Headers.GetValues(RequestHeaderKeys.RequestId).Should()
            .NotBeNull()
            .And.Contain(requestId);

        response.Headers.GetValues(RequestHeaderKeys.CorrelationId).Should()
            .NotBeNull()
            .And.Contain(correlationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static IHost StartHostWithException(Exception exception)
    {
        return HostProvider.StartHostWithEndpoint(_ => throw exception,
            addServices: services => services.AddSingleton(new JsonSerializerOptions().ForFhirExtended()),
            configureApp: app => app.UseMiddleware<ResponseMiddleware>());
    }

    private static IHost StartHost()
    {
        return HostProvider.StartHostWithEndpoint(_ => Task.CompletedTask,
            addServices: services => services.AddSingleton(new JsonSerializerOptions().ForFhirExtended()),
            configureApp: app => app.UseMiddleware<ResponseMiddleware>());
    }
}
