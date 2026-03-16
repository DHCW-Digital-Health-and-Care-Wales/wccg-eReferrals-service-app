using FluentAssertions;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Errors;

namespace NWRI.eReferralsService.Unit.Tests.Errors;

public class NotSuccessfulWpasApiResponseErrorTests
{
    [Fact]
    public void ShouldCorrectlyCreateNotSuccessfulApiResponseError()
    {
        // Arrange
        const string expectedDisplayMessage = "500: The Receiver has encountered an error processing the request.";

        // Act
        var error = new NotSuccessfulWpasApiResponseError();

        // Assert
        error.Code.Should().Be(FhirHttpErrorCodes.ReceiverServerError);
        error.IssueType.Should().Be(OperationOutcome.IssueType.Exception);
        error.DiagnosticsMessage.Should().Be("WPAS API returned an unsuccessful response.");
        error.Display.Should().Be(expectedDisplayMessage);
    }
}
