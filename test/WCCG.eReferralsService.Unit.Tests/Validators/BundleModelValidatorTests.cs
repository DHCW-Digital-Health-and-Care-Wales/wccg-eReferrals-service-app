using AutoFixture;
using FluentValidation;
using FluentValidation.TestHelper;
using Hl7.Fhir.Model;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Models;
using WCCG.eReferralsService.API.Validators;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Validators;

public class BundleModelValidatorTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    private readonly BundleModelValidator _sut;

    public BundleModelValidatorTests()
    {
        _sut = _fixture.CreateWithFrozen<BundleModelValidator>();
        _sut.ClassLevelCascadeMode = CascadeMode.Continue;

        _fixture.Register(() => new MessageHeader { Id = _fixture.Create<string>() });
        _fixture.Register(() => new ServiceRequest { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Patient { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Encounter { Id = _fixture.Create<string>() });
        _fixture.Register(() => new CarePlan { Id = _fixture.Create<string>() });
        _fixture.Register(() => new HealthcareService { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Organization { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Practitioner { Id = _fixture.Create<string>() });
        _fixture.Register(() => new PractitionerRole { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Consent { Id = _fixture.Create<string>() });
    }

    [Fact]
    public void ShouldContainErrorWhenMessageHeaderNull()
    {
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.MessageHeader)
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.MessageHeader)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(MessageHeader)));
    }

    [Fact]
    public void ShouldContainErrorWhenServiceRequestNull()
    {
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.ServiceRequest)
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ServiceRequest)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(ServiceRequest)));
    }

    [Fact]
    public void ShouldContainErrorWhenPatientNull()
    {
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.Patient)
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Patient)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Patient)));
    }

    [Fact]
    public void ShouldContainErrorWhenEncounterNull()
    {
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.Encounter)
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Encounter)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Encounter)));
    }

    [Fact]
    public void ShouldContainErrorWhenCarePlanNull()
    {
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.CarePlan)
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CarePlan)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(CarePlan)));
    }

    [Fact]
    public void ShouldContainErrorWhenHealthcareServiceNull()
    {
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.HealthcareService)
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.HealthcareService)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(HealthcareService)));
    }

    [Fact]
    public void ShouldContainErrorWhenOrganizationsLessThanTwo()
    {
        var model = _fixture.Build<BundleModel>()
            .With(x => x.Organizations, new List<Organization> { new Organization() })
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Organizations)
            .WithErrorMessage(ValidationMessages.MinCardinality(nameof(Organization), 2));
    }

    [Fact]
    public void ShouldContainErrorWhenPractitionersEmpty()
    {
        var model = _fixture.Build<BundleModel>()
            .With(x => x.Practitioners, new List<Practitioner>())
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Practitioners)
            .WithErrorMessage(ValidationMessages.MinCardinality(nameof(Practitioner), 1));
    }

    [Fact]
    public void ShouldContainErrorWhenPractitionerRolesEmpty()
    {
        var model = _fixture.Build<BundleModel>()
            .With(x => x.PractitionerRoles, new List<PractitionerRole>())
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PractitionerRoles)
            .WithErrorMessage(ValidationMessages.MinCardinality(nameof(PractitionerRole), 1));
    }

    [Fact]
    public void ShouldContainErrorWhenConsentsEmpty()
    {
        var model = _fixture.Build<BundleModel>()
            .With(x => x.Consents, new List<Consent>())
            .Create();

        var result = _sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Consents)
            .WithErrorMessage(ValidationMessages.MinCardinality(nameof(Consent), 1));
    }
}
