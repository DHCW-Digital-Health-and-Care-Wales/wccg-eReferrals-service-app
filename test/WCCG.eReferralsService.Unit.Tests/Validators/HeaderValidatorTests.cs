using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Validators;
using WCCG.eReferralsService.Unit.Tests.Extensions;

namespace WCCG.eReferralsService.Unit.Tests.Validators;

public class HeaderValidatorTests
{
    private readonly IFixture _fixture = new Fixture().WithCustomizations();

    private readonly HeaderValidator _sut;

    public HeaderValidatorTests()
    {
        _sut = _fixture.CreateWithFrozen<HeaderValidator>();
    }

    [Fact]
    public void ValidateHeadersShouldNotThrowWhenAllRequiredHeadersPresent()
    {
        //Arrange
        var headersDictionary = new HeaderDictionary();
        foreach (var header in RequestHeaderKeys.GetAllRequired())
        {
            headersDictionary.Add(header, _fixture.Create<string>());
        }

        //Act
        var action = () => _sut.ValidateHeaders(headersDictionary);

        //Assert
        action.Should().NotThrow<Exception>();
    }

    [Fact]
    public void ValidateHeadersShouldThrowWhenAnyRequiredHeadersMissing()
    {
        //Arrange
        var headersDictionary = _fixture.Create<HeaderDictionary>();

        var expectedMissingHeaderPart = string.Join(',', RequestHeaderKeys.GetAllRequired());
        //Act
        var action = () => _sut.ValidateHeaders(headersDictionary);

        //Assert
        action.Should().Throw<MissingRequiredHeaderException>()
            .Which.Message.Should().Contain(expectedMissingHeaderPart);
    }
}
