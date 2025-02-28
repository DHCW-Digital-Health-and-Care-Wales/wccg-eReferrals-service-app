using AutoFixture;
using FluentAssertions;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Exceptions;

public class MissingRequiredHeaderExceptionTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void ShouldCorrectlyCreateMissingRequiredHeaderException()
    {
        //Arrange
        var headerNames = _fixture.CreateMany<string>().ToList();
        var expectedMessage = $"Missing required header(s): {string.Join(',', headerNames)}";

        //Act
        var exception = new MissingRequiredHeaderException(headerNames);

        //Assert
        exception.IssueType.Should().Be(OperationOutcome.IssueType.Required);
        exception.Message.Should().Be(expectedMessage);
        exception.Errors.Should().AllSatisfy(error => { error.Should().BeOfType<HeaderValidationError>(); });
    }
}
