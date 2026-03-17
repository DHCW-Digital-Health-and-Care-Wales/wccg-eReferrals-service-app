using System.Net;
using AutoFixture;
using FluentAssertions;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Errors;
using NWRI.eReferralsService.API.Exceptions;
using NWRI.eReferralsService.Unit.Tests.Extensions;

namespace NWRI.eReferralsService.Unit.Tests.Exceptions;

public class NotSuccessfulWpasApiCallExceptionTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void ShouldMapInternalServerErrorToReceiverServerError()
    {
        // Arrange
        const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        var errorMessage = _fixture.Create<string>();
        var expectedMessage = $"WPAS API call failed with status code: {(int)statusCode}. Raw content: {errorMessage}";

        // Act
        var exception = new NotSuccessfulWpasApiCallException(statusCode, errorMessage);

        // Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().ContainSingle();
        exception.Errors.Should().AllSatisfy(e => e.Should().BeOfType<ReceiverServerError>()
            .Which.Code.Should().Be(FhirHttpErrorCodes.ReceiverServerError));
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.NotFound)]
    public void ShouldMapNon5xxStatusCodesToReceiverServerError(HttpStatusCode statusCode)
    {
        // Arrange
        var errorMessage = _fixture.Create<string>();
        var expectedMessage = $"WPAS API call failed with status code: {(int)statusCode}. Raw content: {errorMessage}";

        // Act
        var exception = new NotSuccessfulWpasApiCallException(statusCode, errorMessage);

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().ContainSingle();
        exception.Errors.Should().AllSatisfy(e => e.Should().BeOfType<ReceiverServerError>()
            .Which.Code.Should().Be(FhirHttpErrorCodes.ReceiverServerError));
    }

    [Theory]
    [InlineData(HttpStatusCode.NotImplemented)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public void ShouldMap5xxStatusCodesAbove500ToServiceUnavailableAndReceiverUnavailableError(HttpStatusCode statusCode)
    {
        // Arrange
        var errorMessage = _fixture.Create<string>();
        var expectedMessage = $"WPAS API call failed with status code: {(int)statusCode}. Raw content: {errorMessage}";

        // Act
        var exception = new NotSuccessfulWpasApiCallException(statusCode, errorMessage);

        // Assert
        exception.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().ContainSingle();
        exception.Errors.Should().AllSatisfy(e => e.Should().BeOfType<ReceiverUnavailableError>()
            .Which.Code.Should().Be(FhirHttpErrorCodes.ReceiverUnavailable));
    }
}
