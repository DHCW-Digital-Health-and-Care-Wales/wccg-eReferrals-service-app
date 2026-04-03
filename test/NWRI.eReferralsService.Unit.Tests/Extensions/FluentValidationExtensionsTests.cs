using FluentValidation;
using FluentValidation.TestHelper;
using NWRI.eReferralsService.API.Extensions;

namespace NWRI.eReferralsService.Unit.Tests.Extensions;

public class FluentValidationExtensionsTests
{
    private class TestModel
    {
        public string? Url { get; init; }
    }

    private class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Url).ValidHttpUrl().WithMessage("Must be a valid URL");
        }
    }

    private readonly TestModelValidator _sut = new();

    [Theory]
    [InlineData("not-a-valid-url", false)]
    [InlineData("http://", false)]
    [InlineData("just/some/path", false)]
    [InlineData("urn:uuid:d5ffd0cd-ec7e-48a1-84f1-91f4c0eb8fc5", false)]
    [InlineData("https://bars.wales.nhs.uk", true)]
    [InlineData("http://example.com/api", true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(null, false)]
    public void ValidHttpUrlShouldValidateCorrectly(string? url, bool isValid)
    {
        var model = new TestModel { Url = url };
        var result = _sut.TestValidate(model);

        if (isValid)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Url);
        }
    }
}

