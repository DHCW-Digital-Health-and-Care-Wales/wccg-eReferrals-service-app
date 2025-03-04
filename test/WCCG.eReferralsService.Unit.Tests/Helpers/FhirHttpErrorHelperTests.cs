using AutoFixture;
using FluentAssertions;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Helpers;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Helpers;

public class FhirHttpErrorHelperTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Theory]
    [InlineData(FhirHttpErrorCodes.SenderBadRequest, "400: The API was unable to process the request.")]
    [InlineData(FhirHttpErrorCodes.ReceiverServerError, "500: The Receiver has encountered an error processing the request.")]
    public void GetDisplayMessageByCodeShouldReturnCorrectMessages(string code, string expectedMessage)
    {
        //Act
        var message = FhirHttpErrorHelper.GetDisplayMessageByCode(code);

        //Assert
        message.Should().Be(expectedMessage);
    }

    [Fact]
    public void GetDisplayMessageByCodeShouldReturnEmptyWhenCodeNotFound()
    {
        //Arrange
        var notValidCode = _fixture.Create<string>();

        //Act
        var message = FhirHttpErrorHelper.GetDisplayMessageByCode(notValidCode);

        //Assert
        message.Should().BeEmpty();
    }
}
