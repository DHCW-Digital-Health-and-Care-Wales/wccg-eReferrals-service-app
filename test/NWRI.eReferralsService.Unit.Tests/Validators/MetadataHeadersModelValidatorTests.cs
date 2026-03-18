using System.Text;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Extensions;
using NWRI.eReferralsService.API.Models;
using NWRI.eReferralsService.API.Validators;
using NWRI.eReferralsService.Unit.Tests.Extensions;

namespace NWRI.eReferralsService.Unit.Tests.Validators;

public class MetadataHeadersModelValidatorTests
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions().ForFhirExtended();
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    private readonly MetadataHeadersModelValidator _sut;

    public MetadataHeadersModelValidatorTests()
    {
        var fhirHeaderValueValidator = new FhirHeaderValueValidator(_jsonSerializerOptions);

        _sut = new MetadataHeadersModelValidator(fhirHeaderValueValidator);
        _sut.ClassLevelCascadeMode = CascadeMode.Continue;
        _sut.RuleLevelCascadeMode = CascadeMode.Stop;

        _fixture.Register(() => new Identifier(_fixture.Create<string>(), _fixture.Create<string>()));
        _fixture.Register(() => new Organization { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Device { Id = _fixture.Create<string>() });
        _fixture.Register(() => new PractitionerRole { Id = _fixture.Create<string>() });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldContainErrorWhenEndUserOrganisationEmpty(string? value)
    {
        var model = CreateValidModel();
        model.EndUserOrganisation = value;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.EndUserOrganisation)
            .WithErrorMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.EndUserOrganisation))
            .WithErrorCode(ValidationErrorCode.MissingRequiredHeaderCode.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldContainErrorWhenRequestingSoftwareEmpty(string? value)
    {
        var model = CreateValidModel();
        model.RequestingSoftware = value;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.RequestingSoftware)
            .WithErrorMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.RequestingSoftware))
            .WithErrorCode(ValidationErrorCode.MissingRequiredHeaderCode.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldContainErrorWhenRequestIdEmpty(string? value)
    {
        var model = CreateValidModel();
        model.RequestId = value;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.RequestId)
            .WithErrorMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.RequestId))
            .WithErrorCode(ValidationErrorCode.MissingRequiredHeaderCode.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldContainErrorWhenCorrelationIdEmpty(string? value)
    {
        var model = CreateValidModel();
        model.CorrelationId = value;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.CorrelationId)
            .WithErrorMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.CorrelationId))
            .WithErrorCode(ValidationErrorCode.MissingRequiredHeaderCode.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldContainErrorWhenAcceptEmpty(string? value)
    {
        var model = CreateValidModel();
        model.Accept = value;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.Accept)
            .WithErrorMessage(ValidationMessages.MissingRequiredHeader(RequestHeaderKeys.Accept))
            .WithErrorCode(ValidationErrorCode.MissingRequiredHeaderCode.ToString());
    }

    [Fact]
    public void ShouldNotContainErrorWhenUseContextMissing()
    {
        var model = CreateValidModel();
        model.UseContext = null;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.UseContext);
    }

    [Fact]
    public void ShouldNotContainErrorWhenTargetIdentifierMissing()
    {
        var model = CreateValidModel();
        model.TargetIdentifier = null;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.TargetIdentifier);
    }

    [Fact]
    public void ShouldContainErrorWhenTargetIdentifierPresentAndInvalid()
    {
        var model = CreateValidModel();
        model.TargetIdentifier = "invalid";

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.TargetIdentifier)
            .WithErrorMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.TargetIdentifier, nameof(Identifier)))
            .WithErrorCode(ValidationErrorCode.InvalidHeaderCode.ToString());
    }

    [Fact]
    public void ShouldNotContainErrorWhenTargetIdentifierPresentAndValid()
    {
        var model = CreateValidModel();
        model.TargetIdentifier = CreateValidBase64<Identifier>();

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.TargetIdentifier);
    }

    [Fact]
    public void ShouldContainErrorWhenRequestingPractitionerPresentAndInvalid()
    {
        var model = CreateValidModel();
        model.RequestingPractitioner = "invalid";

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldHaveValidationErrorFor(x => x.RequestingPractitioner)
            .WithErrorMessage(ValidationMessages.InvalidFhirObject(RequestHeaderKeys.RequestingPractitioner, nameof(PractitionerRole)))
            .WithErrorCode(ValidationErrorCode.InvalidHeaderCode.ToString());
    }

    [Fact]
    public void ShouldNotContainErrorWhenRequestingPractitionerMissing()
    {
        var model = CreateValidModel();
        model.RequestingPractitioner = null;

        var validationResult = _sut.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.RequestingPractitioner);
    }

    [Fact]
    public void ShouldBeValidWhenRequiredMetadataHeadersArePresent()
    {
        var model = CreateValidModel();

        var validationResult = _sut.TestValidate(model);

        validationResult.IsValid.Should().BeTrue();
    }

    private HeadersModel CreateValidModel()
    {
        return new HeadersModel
        {
            TargetIdentifier = null,
            EndUserOrganisation = CreateValidBase64<Organization>(),
            RequestingSoftware = CreateValidBase64<Device>(),
            RequestId = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            UseContext = null,
            Accept = "application/fhir+json;version=1.2.0",
            RequestingPractitioner = null
        };
    }

    private string CreateValidBase64<T>()
    {
        var obj = _fixture.Create<T>();
        var json = JsonSerializer.Serialize(obj, _jsonSerializerOptions);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }
}
