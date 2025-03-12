using System.Net;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Exceptions;

public class NotSuccessfulApiCallExceptionTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void ShouldCorrectlyCreateNotSuccessfulApiCallExceptionForValidationErrors()
    {
        //Arrange
        var statusCode = _fixture.Create<HttpStatusCode>();
        var errorMessages = _fixture.CreateMany<string>().ToList();
        var errorMessagesJson = JsonSerializer.Serialize(errorMessages);

        var problemDetails = _fixture.Build<ProblemDetails>()
            .With(x => x.Extensions, new Dictionary<string, object?> { { "validationErrors", errorMessagesJson } })
            .Create();

        var errors = errorMessages.Select(e => new NotSuccessfulApiResponseError(FhirHttpErrorCodes.ReceiverBadRequest, e));
        var expectedMessage = $"API cal returned: {statusCode}. {string.Join(';', errors.Select(x => x.DiagnosticsMessage))}.";

        //Act
        var exception = new NotSuccessfulApiCallException(statusCode, problemDetails);

        //Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().AllSatisfy(e => e.Should().BeOfType<NotSuccessfulApiResponseError>());
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, FhirHttpErrorCodes.ReceiverBadRequest)]
    [InlineData(HttpStatusCode.InternalServerError, FhirHttpErrorCodes.ReceiverUnavailable)]
    public void ShouldCorrectlyCreateNotSuccessfulApiCallExceptionForRegularError(HttpStatusCode statusCode, string errorCode)
    {
        //Arrange
        var errorMessage = _fixture.Create<string>();

        var problemDetails = _fixture.Build<ProblemDetails>()
            .With(x => x.Detail, errorMessage)
            .Create();

        var expectedMessage = $"API cal returned: {statusCode}. Receiver error. {errorMessage}.";

        //Act
        var exception = new NotSuccessfulApiCallException(statusCode, problemDetails);

        //Assert
        exception.StatusCode.Should().Be(statusCode);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().AllSatisfy(e => e.Should().BeOfType<NotSuccessfulApiResponseError>().Which.Code.Should().Be(errorCode));
    }
}
