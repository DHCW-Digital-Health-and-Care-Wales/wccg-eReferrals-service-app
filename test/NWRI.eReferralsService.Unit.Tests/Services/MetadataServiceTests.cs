using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.EventLogging.Interfaces;
using NWRI.eReferralsService.API.Exceptions;
using NWRI.eReferralsService.API.Models;
using NWRI.eReferralsService.API.Services;
using NWRI.eReferralsService.API.Validators;
using NWRI.eReferralsService.Unit.Tests.Extensions;

namespace NWRI.eReferralsService.Unit.Tests.Services;

public class MetadataServiceTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public async Task GetMetadataAsyncShouldValidateHeaders()
    {
        // Arrange
        var headers = CreateHeaders();
        var expectedModel = HeadersModel.FromHeaderDictionary(headers);

        var modelArgs = new List<HeadersModel>();
        _fixture.Mock<IMetadataHeadersValidator>()
            .Setup(x => x.ValidateAsync(Capture.In(modelArgs), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _fixture.Mock<ICapabilityStatementService>()
            .Setup(x => x.GetCapabilityStatementAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.Create<string>());

        var sut = CreateMetadataService();

        // Act
        await sut.GetMetadataAsync(headers, CancellationToken.None);

        // Assert
        modelArgs[0].Should().BeEquivalentTo(expectedModel);
        _fixture.Mock<IMetadataHeadersValidator>()
            .Verify(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task GetMetadataAsyncShouldThrowWhenInvalidHeaders()
    {
        // Arrange
        var headers = CreateHeaders();

        var validationFailures = _fixture.CreateMany<ValidationFailure>().ToList();
        var validationResult = _fixture.Build<ValidationResult>()
            .With(x => x.Errors, validationFailures)
            .Create();

        _fixture.Mock<IMetadataHeadersValidator>()
            .Setup(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sut = CreateMetadataService();

        // Act
        var action = async () => await sut.GetMetadataAsync(headers, CancellationToken.None);

        // Assert
        (await action.Should().ThrowAsync<HeaderValidationException>())
            .Which.Message.Should().Contain(string.Join(';', validationFailures.Select(x => x.ErrorMessage)));
    }

    [Fact]
    public async Task GetMetadataAsyncShouldNotCallCapabilityStatementServiceWhenInvalidHeaders()
    {
        // Arrange
        var headers = CreateHeaders();

        var validationFailures = _fixture.CreateMany<ValidationFailure>().ToList();
        var validationResult = _fixture.Build<ValidationResult>()
            .With(x => x.Errors, validationFailures)
            .Create();

        _fixture.Mock<IMetadataHeadersValidator>()
            .Setup(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sut = CreateMetadataService();

        // Act
        var action = async () => await sut.GetMetadataAsync(headers, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<HeaderValidationException>();

        _fixture.Mock<ICapabilityStatementService>()
            .Verify(x => x.GetCapabilityStatementAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetMetadataAsyncShouldReturnCapabilityStatementJson()
    {
        // Arrange
        var headers = CreateHeaders();
        var outputJson = _fixture.Create<string>();

        _fixture.Mock<IMetadataHeadersValidator>()
            .Setup(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _fixture.Mock<ICapabilityStatementService>()
            .Setup(x => x.GetCapabilityStatementAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputJson);

        var sut = CreateMetadataService();

        // Act
        var result = await sut.GetMetadataAsync(headers, CancellationToken.None);

        // Assert
        result.Should().Be(outputJson);
    }

    [Fact]
    public async Task GetMetadataAsyncShouldCallCapabilityStatementServiceWhenHeadersValid()
    {
        // Arrange
        var headers = CreateHeaders();

        _fixture.Mock<IMetadataHeadersValidator>()
            .Setup(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _fixture.Mock<ICapabilityStatementService>()
            .Setup(x => x.GetCapabilityStatementAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_fixture.Create<string>());

        var sut = CreateMetadataService();

        // Act
        await sut.GetMetadataAsync(headers, CancellationToken.None);

        // Assert
        _fixture.Mock<ICapabilityStatementService>()
            .Verify(x => x.GetCapabilityStatementAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private MetadataService CreateMetadataService()
    {
        return new MetadataService(
            _fixture.Mock<IMetadataHeadersValidator>().Object,
            _fixture.Mock<ICapabilityStatementService>().Object,
             _fixture.Mock<IEventLogger>().Object);
    }

    private static IHeaderDictionary CreateHeaders()
    {
        return new HeaderDictionary
        {
            { RequestHeaderKeys.EndUserOrganisation, "end-user-organisation" },
            { RequestHeaderKeys.RequestingSoftware, "requesting-software" },
            { RequestHeaderKeys.RequestId, Guid.NewGuid().ToString() },
            { RequestHeaderKeys.CorrelationId, Guid.NewGuid().ToString() },
            { RequestHeaderKeys.Accept, "application/fhir+json;version=1.2.0" }
        };
    }
}
