using Microsoft.AspNetCore.Mvc;
using WCCG.eReferralsService.API.Extensions;
using WCCG.eReferralsService.API.Services;
using WCCG.eReferralsService.API.Swagger;
using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Controllers;

[ApiController]
public class ReferralsController : ControllerBase
{
    private readonly IReferralService _referralService;
    private readonly ILogger<ReferralsController> _logger;

    public ReferralsController(IReferralService referralService, ILogger<ReferralsController> logger)
    {
        _referralService = referralService;
        _logger = logger;
    }

    [HttpPost("/$process-message")]
    [SwaggerProcessMessageRequest]
    public async Task<IActionResult> CreateReferral()
    {
        _logger.CalledMethod(nameof(CreateReferral));

        using var reader = new StreamReader(HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        var outputBundleJson = await _referralService.CreateReferralAsync(HttpContext.Request.Headers, body);

        return new ContentResult
        {
            Content = outputBundleJson,
            StatusCode = 200,
            ContentType = FhirConstants.FhirMediaType
        };
    }

    [HttpGet("ServiceRequest/{id}")]
    [SwaggerGetReferralRequest]
    public async Task<IActionResult> GetReferral(string? id)
    {
        _logger.CalledMethod(nameof(GetReferral));

        var outputBundleJson = await _referralService.GetReferralAsync(HttpContext.Request.Headers, id);

        return new ContentResult
        {
            Content = outputBundleJson,
            StatusCode = 200,
            ContentType = FhirConstants.FhirMediaType
        };
    }
}
