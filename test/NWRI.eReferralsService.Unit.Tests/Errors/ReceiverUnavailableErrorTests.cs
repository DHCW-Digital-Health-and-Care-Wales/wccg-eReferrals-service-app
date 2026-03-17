using FluentAssertions;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Errors;

namespace NWRI.eReferralsService.Unit.Tests.Errors;

public class ReceiverUnavailableErrorTests
{
    [Fact]
    public void ShouldCorrectlyCreateReceiverUnavailableError()
    {
        // Arrange
        const string expectedDisplayMessage = "503: The Receiver is currently unavailable.";

        // Act
        var error = new ReceiverUnavailableError();

        // Assert
        error.Code.Should().Be(FhirHttpErrorCodes.ReceiverUnavailable);
        error.IssueType.Should().Be(OperationOutcome.IssueType.Exception);
        error.DiagnosticsMessage.Should().Be("WPAS API returned an unsuccessful response.");
        error.Display.Should().Be(expectedDisplayMessage);
    }
}
