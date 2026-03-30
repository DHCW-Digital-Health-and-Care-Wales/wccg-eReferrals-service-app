using Microsoft.AspNetCore.Mvc;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.Extensions.Logger;
using NWRI.eReferralsService.API.Middleware;
using NWRI.eReferralsService.API.Services;
using NWRI.eReferralsService.API.Swagger.Attributes;

namespace NWRI.eReferralsService.API.Controllers;

[ApiController]
[AuditLogRequest]
public sealed class MetadataController : ControllerBase
{
    private readonly IMetadataService _metadataService;
    private readonly ILogger<MetadataController> _logger;

    public MetadataController(IMetadataService metadataService, ILogger<MetadataController> logger)
    {
        _metadataService = metadataService;
        _logger = logger;
    }

    [HttpGet("/metadata")]
    [SwaggerGetMetadataRequest]
    public async Task<IActionResult> GetMetadata(CancellationToken cancellationToken)
    {
        _logger.CalledMethod(nameof(GetMetadata));

        var json = await _metadataService.GetMetadataAsync(Request.Headers, cancellationToken);

        return new ContentResult
        {
            Content = json,
            StatusCode = StatusCodes.Status200OK,
            ContentType = FhirConstants.FhirMediaType
        };
    }
}
