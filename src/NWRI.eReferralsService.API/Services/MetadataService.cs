using NWRI.eReferralsService.API.EventLogging;
using NWRI.eReferralsService.API.EventLogging.Interfaces;
using NWRI.eReferralsService.API.Exceptions;
using NWRI.eReferralsService.API.Models;
using NWRI.eReferralsService.API.Validators;

namespace NWRI.eReferralsService.API.Services;
public sealed class MetadataService : IMetadataService
{
    private readonly IMetadataHeadersValidator _metadataHeadersValidator;
    private readonly ICapabilityStatementService _capabilityStatementService;
    private readonly IEventLogger _eventLogger;

    public MetadataService(
        IMetadataHeadersValidator metadataHeadersValidator,
        ICapabilityStatementService capabilityStatementService,
        IEventLogger eventLogger)
    {
        _metadataHeadersValidator = metadataHeadersValidator;
        _capabilityStatementService = capabilityStatementService;
        _eventLogger = eventLogger;
    }

    public async Task<string> GetMetadataAsync(IHeaderDictionary headers, CancellationToken cancellationToken)
    {
        var headersModel = HeadersModel.FromHeaderDictionary(headers);
        var validationResult = await _metadataHeadersValidator.ValidateAsync(headersModel, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new HeaderValidationException(validationResult.Errors);
        }
        _eventLogger.Audit(new EventCatalogue.HeadersValidated());

        return await _capabilityStatementService.GetCapabilityStatementAsync(cancellationToken);
    }
}
