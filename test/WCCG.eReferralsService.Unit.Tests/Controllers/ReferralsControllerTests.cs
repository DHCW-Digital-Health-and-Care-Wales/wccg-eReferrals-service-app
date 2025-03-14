using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Controllers;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Controllers;

public class ReferralsControllerTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    private readonly ReferralsController _sut;

    public ReferralsControllerTests()
    {
        _fixture.OmitAutoProperties = true;
        _sut = _fixture.CreateWithFrozen<ReferralsController>();

        _fixture.Register<IHeaderDictionary>(() => new HeaderDictionary { { _fixture.Create<string>(), _fixture.Create<string>() } });
    }

    [Fact]
    public async Task CreateReferralShouldCallCreateReferralAsync()
    {
        //Arrange
        var body = _fixture.Create<string>();
        var headers = _fixture.Create<IHeaderDictionary>();

        SetRequestDetails(headers, body);

        var headerArgs = new List<IHeaderDictionary>();
        _fixture.Mock<IReferralService>().Setup(x => x.CreateReferralAsync(Capture.In(headerArgs), It.IsAny<string>()));
        //Act
        await _sut.CreateReferral();

        //Assert
        headerArgs[0].Should().ContainKeys(headers.Keys);
        _fixture.Mock<IReferralService>().Verify(x => x.CreateReferralAsync(It.IsAny<IHeaderDictionary>(), body));
    }

    [Fact]
    public async Task CreateReferralShouldReturn200()
    {
        //Arrange
        var body = _fixture.Create<string>();
        var headers = _fixture.Create<IHeaderDictionary>();
        SetRequestDetails(headers, body);

        var outputBundleJson = _fixture.Create<string>();

        _fixture.Mock<IReferralService>().Setup(x => x.CreateReferralAsync(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
            .ReturnsAsync(outputBundleJson);

        //Act
        var result = await _sut.CreateReferral();

        //Assert
        var contentResult = result.Should().BeOfType<ContentResult>().Subject;
        contentResult.StatusCode.Should().Be(200);
        contentResult.Content.Should().Be(outputBundleJson);
        contentResult.ContentType.Should().Be(FhirConstants.FhirMediaType);
    }

    [Fact]
    public async Task GetReferralShouldCallGetReferralAsync()
    {
        //Arrange
        var id = _fixture.Create<string>();
        var headers = _fixture.Create<IHeaderDictionary>();

        SetRequestDetails(headers);

        var headerArgs = new List<IHeaderDictionary>();
        _fixture.Mock<IReferralService>().Setup(x => x.GetReferralAsync(Capture.In(headerArgs), It.IsAny<string>()));
        //Act
        await _sut.GetReferral(id);

        //Assert
        headerArgs[0].Should().ContainKeys(headers.Keys);
        _fixture.Mock<IReferralService>().Verify(x => x.GetReferralAsync(It.IsAny<IHeaderDictionary>(), id));
    }

    [Fact]
    public async Task GetReferralShouldReturn200()
    {
        //Arrange
        var id = _fixture.Create<string>();
        var headers = _fixture.Create<IHeaderDictionary>();

        SetRequestDetails(headers);

        var outputBundleJson = _fixture.Create<string>();

        _fixture.Mock<IReferralService>().Setup(x => x.GetReferralAsync(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
            .ReturnsAsync(outputBundleJson);

        //Act
        var result = await _sut.GetReferral(id);

        //Assert
        var contentResult = result.Should().BeOfType<ContentResult>().Subject;
        contentResult.StatusCode.Should().Be(200);
        contentResult.Content.Should().Be(outputBundleJson);
        contentResult.ContentType.Should().Be(FhirConstants.FhirMediaType);
    }

    private void SetRequestDetails(IHeaderDictionary headerDictionary, string? body = null)
    {
        _sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        foreach (var keyValuePair in headerDictionary)
        {
            _sut.Request.Headers.Add(keyValuePair);
        }

        if (body is null)
        {
            return;
        }

        _sut.ControllerContext.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        _sut.ControllerContext.HttpContext.Request.ContentLength = body.Length;
    }
}
