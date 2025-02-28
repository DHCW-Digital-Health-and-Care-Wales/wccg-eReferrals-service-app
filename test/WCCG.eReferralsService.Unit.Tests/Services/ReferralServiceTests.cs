using AutoFixture;
using FluentAssertions;
using Moq;
using WCCG.eReferralsService.API.ApiClients;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Services;

public class ReferralServiceTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    private readonly ReferralService _sut;

    public ReferralServiceTests()
    {
        _sut = _fixture.CreateWithFrozen<ReferralService>();
    }

    [Fact]
    public async Task CreateReferralAsyncShouldCallApiMethod()
    {
        //Arrange
        var bundleJson = _fixture.Create<string>();

        //Act
        await _sut.CreateReferralAsync(bundleJson);

        //Assert
        _fixture.Mock<IPasReferralsApiClient>().Verify(x => x.CreateReferralAsync(bundleJson));
    }

    [Fact]
    public async Task CreateReferralAsyncShouldReturnOutputBundleJson()
    {
        //Arrange
        var outputBundleJson = _fixture.Create<string>();

        _fixture.Mock<IPasReferralsApiClient>().Setup(x => x.CreateReferralAsync(It.IsAny<string>()))
            .ReturnsAsync(outputBundleJson);

        //Act
        var result = await _sut.CreateReferralAsync(_fixture.Create<string>());

        //Assert
        result.Should().Be(outputBundleJson);
    }
}
