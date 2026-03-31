using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Controllers;
using NWRI.eReferralsService.API.Services;
using NWRI.eReferralsService.Unit.Tests.Extensions;

namespace NWRI.eReferralsService.Unit.Tests.Controllers;

public class MetadataControllerTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    private readonly MetadataController _sut;

    public MetadataControllerTests()
    {
        _fixture.OmitAutoProperties = true;
        _sut = _fixture.CreateWithFrozen<MetadataController>();

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task GetMetadataShouldReturn200WithFhirContentTypeAndJson()
    {
        // Arrange
        var outputJson = _fixture.Create<string>();

        _fixture.Mock<IMetadataService>()
            .Setup(x => x.GetMetadataAsync(It.IsAny<IHeaderDictionary>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputJson);

        // Act
        var result = await _sut.GetMetadata(CancellationToken.None);

        // Assert
        var contentResult = result.Should().BeOfType<ContentResult>().Subject;
        contentResult.StatusCode.Should().Be(200);
        contentResult.Content.Should().Be(outputJson);
        contentResult.ContentType.Should().Be(FhirConstants.FhirMediaType);
    }

    [Fact]
    public async Task GetMetadataShouldPassRequestHeadersToService()
    {
        // Arrange
        var outputJson = _fixture.Create<string>();

        _fixture.Mock<IMetadataService>()
            .Setup(x => x.GetMetadataAsync(It.IsAny<IHeaderDictionary>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outputJson);

        // Act
        await _sut.GetMetadata(CancellationToken.None);

        // Assert
        _fixture.Mock<IMetadataService>().Verify(
            x => x.GetMetadataAsync(
                It.Is<IHeaderDictionary>(h => h == _sut.Request.Headers),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMetadataShouldPropagateExceptionsFromService()
    {
        // Arrange
        var ex = new FileNotFoundException(_fixture.Create<string>());

        _fixture.Mock<IMetadataService>()
            .Setup(x => x.GetMetadataAsync(It.IsAny<IHeaderDictionary>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        // Act
        Func<Task> act = async () => await _sut.GetMetadata(CancellationToken.None);

        // Assert
        (await act.Should().ThrowAsync<FileNotFoundException>())
            .Which.Message.Should().Be(ex.Message);
    }
}
