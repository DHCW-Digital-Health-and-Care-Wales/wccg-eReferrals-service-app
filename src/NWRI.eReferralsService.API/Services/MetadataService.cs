using FluentValidation;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.EventLogging;
using NWRI.eReferralsService.API.EventLogging.Interfaces;
using NWRI.eReferralsService.API.Exceptions;
using NWRI.eReferralsService.API.Models;

namespace NWRI.eReferralsService.API.Services;

public sealed class MetadataService : IMetadataService
{
    private readonly IValidator<HeadersModel> _metadataHeadersValidator;
    private readonly ICapabilityStatementService _capabilityStatementService;
    private readonly IEventLogger _eventLogger;

    public MetadataService(
       [FromKeyedServices(HeaderValidatorKeys.Referral)] IValidator<HeadersModel> metadataHeadersValidator,
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
