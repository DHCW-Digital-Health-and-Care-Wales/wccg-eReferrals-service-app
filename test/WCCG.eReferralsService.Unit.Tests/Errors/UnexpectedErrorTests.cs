using AutoFixture;
using FluentAssertions;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Errors;

public class UnexpectedErrorTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void ShouldCorrectlyCreateUnexpectedError()
    {
        //Arrange
        var exception = _fixture.Create<Exception>();
        var expectedDetailsMessage = $"Unexpected error: {exception.Message}";
        const string expectedDisplayMessage = "500: The Receiver has encountered an error processing the request.";

        //Act
        var error = new UnexpectedError(exception);

        //Assert
        error.Code.Should().Be(FhirHttpErrorCodes.ReceiverServerError);
        error.DiagnosticsMessage.Should().Be(expectedDetailsMessage);
        error.System.Should().Be(FhirConstants.HttpErrorCodesSystem);
        error.Display.Should().Be(expectedDisplayMessage);
    }
}
