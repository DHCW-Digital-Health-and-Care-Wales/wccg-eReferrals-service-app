using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Validators;

namespace WCCG.eReferralsService.Unit.Tests.Validators;

public class FhirBundleProfileValidatorTests
{
    [Fact]
    public void ValidateShouldReturnEmptyOperationOutcomeWhenDisabled()
    {
        // Arrange
        var config = Options.Create(new FhirValidationConfig
        {
            Enabled = false,
            PackagePaths = []
        });

        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.SetupGet(x => x.ContentRootPath).Returns("/tmp");

        var sut = new FhirBundleProfileValidator(config, hostEnvironment.Object, NullLogger<FhirBundleProfileValidator>.Instance);

        // Act
        var outcome = sut.Validate(new Bundle { Type = Bundle.BundleType.Message });

        // Assert
        outcome.Id.Should().NotBeNullOrWhiteSpace();
        outcome.Meta.Should().NotBeNull();
        outcome.Meta!.Profile.Should().Contain(FhirConstants.OperationOutcomeProfile);
        outcome.Issue.Should().BeEmpty();
    }

    [Fact]
    public void ValidateShouldThrowWhenEnabledAndNoPackagePathsConfigured()
    {
        // Arrange
        var config = Options.Create(new FhirValidationConfig
        {
            Enabled = true,
            PackagePaths = []
        });

        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.SetupGet(x => x.ContentRootPath).Returns("/tmp");

        var sut = new FhirBundleProfileValidator(config, hostEnvironment.Object, NullLogger<FhirBundleProfileValidator>.Instance);

        // Act
        var action = () => sut.Validate(new Bundle { Type = Bundle.BundleType.Message });

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*no package paths are configured*");
    }

    [Fact]
    public void ValidateShouldThrowWhenEnabledAndConfiguredPackageFileDoesNotExist()
    {
        // Arrange
        var config = Options.Create(new FhirValidationConfig
        {
            Enabled = true,
            PackagePaths = ["does-not-exist.tgz"]
        });

        var hostEnvironment = new Mock<IHostEnvironment>();
        hostEnvironment.SetupGet(x => x.ContentRootPath).Returns("/tmp");

        var sut = new FhirBundleProfileValidator(config, hostEnvironment.Object, NullLogger<FhirBundleProfileValidator>.Instance);

        // Act
        var action = () => sut.Validate(new Bundle { Type = Bundle.BundleType.Message });

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*do not exist*");
    }
}
