using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using RichardSzalay.MockHttp;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Models;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.Unit.Tests.Extensions;
using Task = System.Threading.Tasks.Task;

namespace WCCG.eReferralsService.Unit.Tests.Services;

public class ReferralServiceTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();
    private readonly PasReferralsApiConfig _pasReferralsApiConfig;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions().ForFhirExtended();

    public ReferralServiceTests()
    {
        _pasReferralsApiConfig = _fixture.Create<PasReferralsApiConfig>();
        _fixture.Mock<IOptions<PasReferralsApiConfig>>().SetupGet(x => x.Value).Returns(_pasReferralsApiConfig);

        _fixture.Register(() => new Bundle
        {
            Id = _fixture.Create<string>(),
            Type = Bundle.BundleType.Message
        });

        _fixture.Register<IHeaderDictionary>(() => new HeaderDictionary { { _fixture.Create<string>(), _fixture.Create<string>() } });
    }

    [Fact]
    public async Task CreateReferralAsyncShouldValidateHeaders()
    {
        //Arrange
        var bundle = _fixture.Create<Bundle>();
        var bundleJson = JsonSerializer.Serialize(bundle, _jsonSerializerOptions);
        var headers = _fixture.Create<IHeaderDictionary>();

        var expectedModel = HeadersModel.FromHeaderDictionary(headers);

        var modelArgs = new List<HeadersModel>();
        _fixture.Mock<IValidator<HeadersModel>>().Setup(x => x.ValidateAsync(Capture.In(modelArgs), It.IsAny<CancellationToken>()));

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, $"/{_pasReferralsApiConfig.CreateReferralEndpoint}")
            .Respond(FhirConstants.FhirMediaType, _fixture.Create<string>());

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://some.com");

        var sut = CreateReferralService(httpClient);

        //Act
        await sut.CreateReferralAsync(headers, bundleJson);

        //Assert
        modelArgs[0].Should().BeEquivalentTo(expectedModel);
        _fixture.Mock<IValidator<HeadersModel>>().Verify(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreateReferralAsyncShouldThrowWhenInvalidHeaders()
    {
        //Arrange
        var bundle = _fixture.Create<Bundle>();
        var bundleJson = JsonSerializer.Serialize(bundle, _jsonSerializerOptions);
        var headers = _fixture.Create<IHeaderDictionary>();

        var validationFailures = _fixture.CreateMany<ValidationFailure>().ToList();
        var validationResult = _fixture.Build<ValidationResult>()
            .With(x => x.Errors, validationFailures)
            .Create();

        _fixture.Mock<IValidator<HeadersModel>>().Setup(x => x.ValidateAsync(It.IsAny<HeadersModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, $"/{_pasReferralsApiConfig.CreateReferralEndpoint}")
            .Respond(FhirConstants.FhirMediaType, _fixture.Create<string>());

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://some.com");

        var sut = CreateReferralService(httpClient);

        //Act
        var action = async () => await sut.CreateReferralAsync(headers, bundleJson);

        //Assert
        (await action.Should().ThrowAsync<HeaderValidationException>())
            .Which.Message.Should().Contain(string.Join(';', validationFailures.Select(x => x.ErrorMessage)));
    }

    [Fact]
    public async Task CreateReferralAsyncShouldValidateBundle()
    {
        //Arrange
        var bundle = _fixture.Create<Bundle>();
        var bundleJson = JsonSerializer.Serialize(bundle, _jsonSerializerOptions);
        var headers = _fixture.Create<IHeaderDictionary>();

        var expectedModel = BundleModel.FromBundle(bundle);

        var modelArgs = new List<BundleModel>();
        _fixture.Mock<IValidator<BundleModel>>().Setup(x => x.ValidateAsync(Capture.In(modelArgs), It.IsAny<CancellationToken>()));

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, $"/{_pasReferralsApiConfig.CreateReferralEndpoint}")
            .Respond(FhirConstants.FhirMediaType, _fixture.Create<string>());

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://some.com");

        var sut = CreateReferralService(httpClient);

        //Act
        await sut.CreateReferralAsync(headers, bundleJson);

        //Assert
        modelArgs[0].Should().BeEquivalentTo(expectedModel);
        _fixture.Mock<IValidator<BundleModel>>().Verify(x => x.ValidateAsync(It.IsAny<BundleModel>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreateReferralAsyncShouldThrowWhenValidationFailed()
    {
        //Arrange
        var bundle = _fixture.Create<Bundle>();
        var bundleJson = JsonSerializer.Serialize(bundle, _jsonSerializerOptions);
        var headers = _fixture.Create<IHeaderDictionary>();

        var validationFailures = _fixture.CreateMany<ValidationFailure>().ToList();
        var validationResult = _fixture.Build<ValidationResult>()
            .With(x => x.Errors, validationFailures)
            .Create();

        _fixture.Mock<IValidator<BundleModel>>().Setup(x => x.ValidateAsync(It.IsAny<BundleModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var sut = CreateReferralService(new MockHttpMessageHandler().ToHttpClient());

        //Act
        var action = async () => await sut.CreateReferralAsync(headers, bundleJson);

        //Assert
        (await action.Should().ThrowAsync<BundleValidationException>())
            .Which.Message.Should().Contain(string.Join(';', validationFailures.Select(x => x.ErrorMessage)));
    }

    [Fact]
    public async Task CreateReferralAsyncShouldReturnOutputBundleJson()
    {
        //Arrange
        var bundleJson = JsonSerializer.Serialize(_fixture.Create<Bundle>(), _jsonSerializerOptions);
        var expectedResponse = _fixture.Create<string>();
        var headers = _fixture.Create<IHeaderDictionary>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, $"/{_pasReferralsApiConfig.CreateReferralEndpoint}")
            .WithContent(bundleJson)
            .WithHeaders(HeaderNames.ContentType, FhirConstants.FhirMediaType)
            .Respond(FhirConstants.FhirMediaType, expectedResponse);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://some.com");

        var sut = CreateReferralService(httpClient);

        //Act
        var result = await sut.CreateReferralAsync(headers, bundleJson);

        //Assert
        result.Should().Be(expectedResponse);
    }

    private ReferralService CreateReferralService(HttpClient httpClient)
    {
        return new ReferralService(
            httpClient,
            _fixture.Mock<IOptions<PasReferralsApiConfig>>().Object,
            _fixture.Mock<IValidator<BundleModel>>().Object,
            _fixture.Mock<IValidator<HeadersModel>>().Object,
            _jsonSerializerOptions
        );
    }
}
