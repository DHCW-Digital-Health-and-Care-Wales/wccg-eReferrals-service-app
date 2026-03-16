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

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.NotFound)]
    public void ShouldCorrectlyCreateNotSuccessfulApiCallException(HttpStatusCode statusCode)
    {
        var errorMessage = _fixture.Create<string>();

        // Arrange
        var expectedMessage = $"API call returned: {(int)statusCode}. Raw content: {errorMessage}";

        // Act
        var exception = new NotSuccessfulWpasApiCallException(statusCode, errorMessage);

        // Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().ContainSingle();
        exception.Errors.Should().AllSatisfy(e => e.Should().BeOfType<NotSuccessfulWpasApiResponseError>()
            .Which.Code.Should().Be(FhirHttpErrorCodes.ReceiverServerError));
    }
}
