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

        _fixture.Register(() => new ServiceRequest { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Patient { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Encounter { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Organization { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Appointment { Id = _fixture.Create<string>() });
        _fixture.Register(() => new Practitioner { Id = _fixture.Create<string>() });
    }

    [Fact]
    public void ShouldContainErrorWhenServiceRequestNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.ServiceRequest)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ServiceRequest)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(ServiceRequest)));
    }

    [Fact]
    public void ShouldNotContainErrorWhenServiceRequestNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ServiceRequest);
    }

    [Fact]
    public void ShouldContainErrorWhenServicePatientNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.Patient)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Patient)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Patient)));
    }

    [Fact]
    public void ShouldNotContainErrorWhenPatientNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Patient);
    }

    [Fact]
    public void ShouldContainErrorWhenEncounterNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.Encounter)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Encounter)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Encounter)));
    }

    [Fact]
    public void ShouldNotContainErrorWhenEncounterNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Encounter);
    }

    [Fact]
    public void ShouldContainErrorWhenAppointmentNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.Appointment)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.Appointment)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Appointment)));
    }

    [Fact]
    public void ShouldNotContainErrorWhenAppointmentNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.Appointment);
    }

    [Fact]
    public void ShouldContainErrorWhenRequestingPractitionerNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.RequestingPractitioner)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.RequestingPractitioner)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Practitioner), FhirConstants.RequestingPractitionerId));
    }

    [Fact]
    public void ShouldNotContainErrorWhenRequestingPractitionerNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.RequestingPractitioner);
    }

    [Fact]
    public void ShouldContainErrorWhenReceivingClinicianPractitionerNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.ReceivingClinicianPractitioner)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ReceivingClinicianPractitioner)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Practitioner), FhirConstants.ReceivingClinicianId));
    }

    [Fact]
    public void ShouldNotContainErrorWhenReceivingClinicianPractitionerNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ReceivingClinicianPractitioner);
    }

    [Fact]
    public void ShouldContainErrorWhenDhaOrganizationNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.DhaOrganization)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.DhaOrganization)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Organization), FhirConstants.DhaCodeId));
    }

    [Fact]
    public void ShouldNotContainErrorWhenDhaOrganizationNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.DhaOrganization);
    }

    [Fact]
    public void ShouldContainErrorWhenReferringPracticeOrganizationNull()
    {
        //Arrange
        var model = _fixture.Build<BundleModel>()
            .Without(x => x.ReferringPracticeOrganization)
            .Create();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldHaveValidationErrorFor(x => x.ReferringPracticeOrganization)
            .WithErrorMessage(ValidationMessages.MissingBundleEntity(nameof(Organization), FhirConstants.ReferringPracticeId));
    }

    [Fact]
    public void ShouldNotContainErrorWhenReferringPracticeOrganizationNotNull()
    {
        //Arrange
        var model = _fixture.Create<BundleModel>();

        //Act
        var validationResult = _sut.TestValidate(model);

        //Assert
        validationResult.ShouldNotHaveValidationErrorFor(x => x.ReferringPracticeOrganization);
    }
}
