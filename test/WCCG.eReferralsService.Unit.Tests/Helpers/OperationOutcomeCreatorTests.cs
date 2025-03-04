using AutoFixture;
using FluentAssertions;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Helpers;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Helpers;

public class OperationOutcomeCreatorTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void CreateOperationOutcomeShouldCreateFromErrors()
    {
        //Arrange
        var issueType = _fixture.Create<OperationOutcome.IssueType>();
        var errors = _fixture.CreateMany<HeaderValidationError>().ToArray<BaseFhirHttpError>();

        var expectedIssues = errors.Select(error => new OperationOutcome.IssueComponent
        {
            Severity = OperationOutcome.IssueSeverity.Error,
            Code = issueType,
            Details = new CodeableConcept(BaseFhirHttpError.System, error.Code, error.Display),
            Diagnostics = error.DiagnosticsMessage
        }).ToList();

        //Act
        var result = OperationOutcomeCreator.CreateOperationOutcome(issueType, errors);

        //Assert
        result.Id.Should().NotBeEmpty();
        result.Meta.Profile.Should().BeEquivalentTo(new List<string> { FhirConstants.OperationOutcomeProfile });
        result.Issue.Should().BeEquivalentTo(expectedIssues);
    }

    [Fact]
    public void CreateOperationOutcomeShouldCreateFromFhirException()
    {
        //Arrange
        var exception = _fixture.Create<MissingRequiredHeaderException>();

        var expectedIssues = exception.Errors.Select(error => new OperationOutcome.IssueComponent
        {
            Severity = OperationOutcome.IssueSeverity.Error,
            Code = exception.IssueType,
            Details = new CodeableConcept(BaseFhirHttpError.System, error.Code, error.Display),
            Diagnostics = error.DiagnosticsMessage
        }).ToList();

        //Act
        var result = OperationOutcomeCreator.CreateOperationOutcome(exception);

        //Assert
        result.Id.Should().NotBeEmpty();
        result.Meta.Profile.Should().BeEquivalentTo(new List<string> { FhirConstants.OperationOutcomeProfile });
        result.Issue.Should().BeEquivalentTo(expectedIssues);
    }
}
