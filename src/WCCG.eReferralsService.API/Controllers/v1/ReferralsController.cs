using Microsoft.AspNetCore.Mvc;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.API.Swagger;
using WCCG.eReferralsService.API.Validators;
using Asp.Versioning;
using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReferralsController : ControllerBase
{
    private readonly IHeaderValidator _headerValidator;
    private readonly IReferralService _referralService;
    private readonly ILogger<ReferralsController> _logger;

    public ReferralsController(
        IHeaderValidator headerValidator,
        IReferralService referralService,
        ILogger<ReferralsController> logger)
    {
        _headerValidator = headerValidator;
        _referralService = referralService;
        _logger = logger;
    }

    [HttpPost("$process-message")]
    [SwaggerProcessMessageRequest]
    public async Task<IActionResult> CreateReferral()
    {
        _logger.CalledMethod(nameof(CreateReferral));

        _headerValidator.ValidateHeaders(HttpContext.Request.Headers);

        using var reader = new StreamReader(HttpContext.Request.Body);
        var bundleJson = await reader.ReadToEndAsync();

        var outputBundleJson = await _referralService.CreateReferralAsync(bundleJson);

        return new ContentResult
        {
            Content = outputBundleJson,
            StatusCode = 200,
            ContentType = FhirConstants.FhirMediaType
        };
    }
}
