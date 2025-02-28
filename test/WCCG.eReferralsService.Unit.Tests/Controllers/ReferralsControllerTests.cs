using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Controllers.v1;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.API.Validators;
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
    }

    [Fact]
    public async Task ShouldCreateValidateHeaders()
    {
        //Arrange
        var bundle = _fixture.Create<string>();
        SetRequestBody(bundle);

        //Act
        await _sut.CreateReferral();

        //Assert
        _fixture.Mock<IHeaderValidator>().Verify(x => x.ValidateHeaders(_sut.HttpContext.Request.Headers));
    }

    [Fact]
    public async Task CreateReferralShouldCallCreateReferralAsync()
    {
        //Arrange
        var body = _fixture.Create<string>();
        SetRequestBody(body);

        //Act
        await _sut.CreateReferral();

        //Assert
        _fixture.Mock<IReferralService>().Verify(x => x.CreateReferralAsync(body));
    }

    [Fact]
    public async Task CreateReferralShouldReturn200()
    {
        //Arrange
        SetRequestBody(_fixture.Create<string>());

        var outputBundleJson = _fixture.Create<string>();

        _fixture.Mock<IReferralService>().Setup(x => x.CreateReferralAsync(It.IsAny<string>()))
            .ReturnsAsync(outputBundleJson);

        //Act
        var result = await _sut.CreateReferral();

        //Assert
        var contentResult = result.Should().BeOfType<ContentResult>().Subject;
        contentResult.StatusCode.Should().Be(200);
        contentResult.Content.Should().Be(outputBundleJson);
        contentResult.ContentType.Should().Be(FhirConstants.FhirMediaType);
    }

    private void SetRequestBody(string value)
    {
        _sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        _sut.ControllerContext.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(value));
        _sut.ControllerContext.HttpContext.Request.ContentLength = value.Length;
    }
}
