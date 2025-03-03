using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using RichardSzalay.MockHttp;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Services;

public class ReferralServiceTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();
    private readonly PasReferralsApiConfig _pasReferralsApiConfig;

    public ReferralServiceTests()
    {
        _pasReferralsApiConfig = _fixture.Create<PasReferralsApiConfig>();
        _fixture.Mock<IOptions<PasReferralsApiConfig>>().SetupGet(x => x.Value).Returns(_pasReferralsApiConfig);
    }

    [Fact]
    public async Task CreateReferralAsyncShouldReturnOutputBundleJson()
    {
        //Arrange
        var bundleJson = _fixture.Create<string>();
        var expectedResponse = _fixture.Create<string>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, $"/{_pasReferralsApiConfig.CreateReferralEndpoint}")
            .WithContent(bundleJson)
            .WithHeaders(HeaderNames.ContentType, FhirConstants.FhirMediaType)
            .Respond(FhirConstants.FhirMediaType, expectedResponse);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://some.com");

        var sut = new ReferralService(httpClient, _fixture.Mock<IOptions<PasReferralsApiConfig>>().Object);

        //Act
        var result = await sut.CreateReferralAsync(bundleJson);

        //Assert
        result.Should().Be(expectedResponse);
    }
}
