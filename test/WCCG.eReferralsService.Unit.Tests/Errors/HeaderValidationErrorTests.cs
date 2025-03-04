using AutoFixture;
using FluentAssertions;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Errors;

public class HeaderValidationErrorTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void ShouldCorrectlyCreateHeaderValidationError()
    {
        //Arrange
        var headerName = _fixture.Create<string>();
        var expectedDetailsMessage = $"Missing required header: {headerName}";
        const string expectedDisplayMessage = "400: The API was unable to process the request.";

        //Act
        var error = new HeaderValidationError(headerName);

        //Assert
        error.Code.Should().Be(FhirHttpErrorCodes.SenderBadRequest);
        error.DiagnosticsMessage.Should().Be(expectedDetailsMessage);
        error.Display.Should().Be(expectedDisplayMessage);
    }
}
