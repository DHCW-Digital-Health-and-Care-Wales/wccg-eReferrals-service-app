using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Hl7.Fhir.Model;
using NWRI.eReferralsService.API.Constants;
using NWRI.eReferralsService.API.EventLogging;
using NWRI.eReferralsService.API.EventLogging.Interfaces;
using NWRI.eReferralsService.API.Exceptions;
using NWRI.eReferralsService.API.Extensions;
using NWRI.eReferralsService.API.Models;
using NWRI.eReferralsService.API.Models.WPAS.Responses;
using Task = System.Threading.Tasks.Task;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace NWRI.eReferralsService.API.Services;

public class ReferralService : IReferralService
{
    private enum ReferralWorkflowAction
    {
        Create,
        Cancel
    }

    private readonly IValidator<HeadersModel> _referralHeadersModelValidator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IEventLogger _eventLogger;
    private readonly IRequestFhirHeadersDecoder _requestFhirHeadersDecoder;
    private readonly IReferralWorkflowProcessor _referralWorkflowProcessor;

    public ReferralService(
        [FromKeyedServices(ServiceKeys.Validators.ReferralHeaders)] IValidator<HeadersModel> referralHeadersModelValidator,
        JsonSerializerOptions jsonSerializerOptions,
        IEventLogger eventLogger,
        IRequestFhirHeadersDecoder requestFhirHeadersDecoder,
        IReferralWorkflowProcessor referralWorkflowProcessor)
    {
        _referralHeadersModelValidator = referralHeadersModelValidator;
        _jsonSerializerOptions = jsonSerializerOptions;
        _eventLogger = eventLogger;
        _requestFhirHeadersDecoder = requestFhirHeadersDecoder;
        _referralWorkflowProcessor = referralWorkflowProcessor;
    }

    public async Task<string> ProcessMessageAsync(IHeaderDictionary headers, string requestBody, CancellationToken cancellationToken)
    {
        var processingStopwatch = Stopwatch.StartNew();

        var headersModel = HeadersModel.FromHeaderDictionary(headers);
        await ValidateHeadersAsync(headersModel);

        _eventLogger.Audit(new EventCatalogue.PayloadValidationStarted());

        var bundle = JsonSerializer.Deserialize<Bundle>(requestBody, _jsonSerializerOptions)!;
        var (workflowAction, serviceRequest) = ResolveWorkflowContext(bundle);

        WpasReferralResponse response = workflowAction switch
        {
            ReferralWorkflowAction.Create => await _referralWorkflowProcessor.ProcessCreateAsync(bundle, cancellationToken),
            ReferralWorkflowAction.Cancel => await _referralWorkflowProcessor.ProcessCancelAsync(bundle, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported workflow action '{workflowAction}'.")
        };

        processingStopwatch.Stop();

        var sourceSystem = _requestFhirHeadersDecoder.GetDecodedSourceSystem(headersModel.RequestingSoftware);
        var userRole = _requestFhirHeadersDecoder.GetDecodedUserRole(headersModel.RequestingPractitioner);

        _eventLogger.Audit(new EventCatalogue.AuditReferralAccepted(sourceSystem, userRole, response.ReferralId,
            processingStopwatch.ElapsedMilliseconds));

        serviceRequest.Id = response.ReferralId;
        return JsonSerializer.Serialize(bundle, _jsonSerializerOptions);
    }

    private static (ReferralWorkflowAction, ServiceRequest) ResolveWorkflowContext(Bundle bundle)
    {
        var serviceRequests = bundle.ResourcesByType<ServiceRequest>().ToList();
        if (serviceRequests.Count == 0)
        {
            throw new RequestParameterValidationException("ServiceRequest", "ServiceRequest is required");
        }

        switch (GetMessageReasonCode(bundle))
        {
            case FhirConstants.BarsMessageReasonNew:
            {
                var createRequest = GetServiceRequestByProfile(serviceRequests, FhirConstants.BarsServiceRequestCreateReferral);
                if (createRequest.Status == RequestStatus.Active)
                {
                    return (ReferralWorkflowAction.Create, createRequest);
                }
                break;
            }
            case FhirConstants.BarsMessageReasonUpdate:
            {
                var cancelRequest = GetServiceRequestByProfile(serviceRequests, FhirConstants.BarsServiceRequestCancelReferral);
                if (cancelRequest.Status is RequestStatus.Revoked or RequestStatus.EnteredInError)
                {
                    return (ReferralWorkflowAction.Cancel, cancelRequest);
                }
                break;
            }
        }

        throw new BundleValidationException([new ValidationFailure("", "Invalid MessageHeader.reason and ServiceRequest profile/status combination.")]);
    }

    private async Task ValidateHeadersAsync(HeadersModel headersModel)
    {
        var headersValidationResult = await _referralHeadersModelValidator.ValidateAsync(headersModel);
        if (!headersValidationResult.IsValid)
        {
            throw new HeaderValidationException(headersValidationResult.Errors);
        }
        _eventLogger.Audit(new EventCatalogue.HeadersValidated());
    }

    private static string GetMessageReasonCode(Bundle bundle)
    {
        var messageReasonCode = bundle.ResourceByType<MessageHeader>()?.Reason?.Coding
            .FirstOrDefault(c => string.Equals(c.System, FhirConstants.BarsMessageReasonSystem, StringComparison.OrdinalIgnoreCase))
            ?.Code;

        return messageReasonCode ?? throw new RequestParameterValidationException("MessageHeader.reason", "MessageHeader.reason.coding.code is required");
    }

    private static ServiceRequest GetServiceRequestByProfile(IList<ServiceRequest> serviceRequests, string profile)
    {
        var serviceRequest = serviceRequests.FirstOrDefault(sr => sr.HasProfile(profile));
        return serviceRequest ?? throw new RequestParameterValidationException("ServiceRequest", $"No ServiceRequest with profile '{profile}' found");
    }
}
