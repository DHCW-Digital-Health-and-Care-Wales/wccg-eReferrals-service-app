using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using WCCG.eReferralsService.API.Models;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Models;

public class BundleModelTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public void ShouldSetAllProperties()
    {
        //Arrange
        var bundleJson = File.ReadAllText("TestData/example-bundle.json");

        var options = new JsonSerializerOptions()
            .ForFhir(ModelInfo.ModelInspector)
            .UsingMode(DeserializerModes.BackwardsCompatible);
        var bundle = JsonSerializer.Deserialize<Bundle>(bundleJson, options)!;

        //Act
        var model = BundleModel.FromBundle(bundle);

        //Assert
        model.ServiceRequest.Should().NotBeNull();
        model.Patient.Should().NotBeNull();
        model.Encounter.Should().NotBeNull();
        model.Appointment.Should().NotBeNull();
        model.RequestingPractitioner.Should().NotBeNull();
        model.ReceivingClinicianPractitioner.Should().NotBeNull();
        model.DhaOrganization.Should().NotBeNull();
        model.ReferringPracticeOrganization.Should().NotBeNull();
    }

    [Fact]
    public void ShouldSetNullIfNotExists()
    {
        //Arrange
        var bundle = new Bundle { Id = _fixture.Create<string>() };

        //Act
        var model = BundleModel.FromBundle(bundle);

        //Assert
        model.ServiceRequest.Should().BeNull();
        model.Patient.Should().BeNull();
        model.Encounter.Should().BeNull();
        model.Appointment.Should().BeNull();
        model.RequestingPractitioner.Should().BeNull();
        model.ReceivingClinicianPractitioner.Should().BeNull();
        model.DhaOrganization.Should().BeNull();
        model.ReferringPracticeOrganization.Should().BeNull();
    }
}
