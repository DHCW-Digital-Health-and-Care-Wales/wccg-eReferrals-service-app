using AutoFixture;
using FluentAssertions;
using Microsoft.Net.Http.Headers;
using RichardSzalay.MockHttp;
using WCCG.eReferralsService.API.ApiClients;
using WCCG.eReferralsService.API.ApiClients.Endpoints;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.ApiClients;

public class PasReferralsApiClientTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    [Fact]
    public async Task CreateReferralAsyncShouldReturnResult()
    {
        //Arrange
        var bundleJson = _fixture.Create<string>();
        var expectedResponse = _fixture.Create<string>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, PasReferralsApiEndpoints.CreateReferralEndpoint)
            .WithContent(bundleJson)
            .WithHeaders(HeaderNames.ContentType, FhirConstants.FhirMediaType)
            .Respond(FhirConstants.FhirMediaType, expectedResponse);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://some.com");

        var sut = new PasReferralsApiClient(httpClient);

        //Act
        var result = await sut.CreateReferralAsync(bundleJson);

        //Assert
        result.Should().Be(expectedResponse);
    }
}
