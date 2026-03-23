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
using NWRI.eReferralsService.API.Validators;
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

    private readonly IReferralHeadersModelValidator _referralHeadersModelValidator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IEventLogger _eventLogger;
    private readonly IRequestFhirHeadersDecoder _requestFhirHeadersDecoder;
    private readonly IReferralWorkflowProcessor _referralWorkflowProcessor;

    public ReferralService(IReferralHeadersModelValidator referralHeadersModelValidator,
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

        var messageReasonCode = GetMessageReasonCode(bundle);
        var serviceRequest = GetReferralServiceRequest(bundle);
        var workflowAction = DetermineReferralWorkflowAction(messageReasonCode, serviceRequest.Status);

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

    private static ReferralWorkflowAction DetermineReferralWorkflowAction(string messageReasonCode, RequestStatus? serviceRequestStatus)
    {
        if (serviceRequestStatus is null)
        {
            throw new RequestParameterValidationException("ServiceRequest.status", "ServiceRequest.status is required");
        }

        if (messageReasonCode == FhirConstants.BarsMessageReasonNew && serviceRequestStatus == RequestStatus.Active)
        {
            return ReferralWorkflowAction.Create;
        }

        if (messageReasonCode == FhirConstants.BarsMessageReasonUpdate &&
            serviceRequestStatus is RequestStatus.Revoked or RequestStatus.EnteredInError)
        {
            return ReferralWorkflowAction.Cancel;
        }

        throw new BundleValidationException([new ValidationFailure("", "Invalid MessageHeader.reason and ServiceRequest.status combination.")]);
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

    private static ServiceRequest GetReferralServiceRequest(Bundle bundle)
    {
        var matching = bundle.ResourcesByProfile<ServiceRequest>(FhirConstants.BarsServiceRequestReferral).ToList();
        if (matching.Count == 0)
        {
            throw new RequestParameterValidationException("ServiceRequest", $"No ServiceRequest with profile '{FhirConstants.BarsServiceRequestReferral}' found in the request bundle.");
        }
        if (matching.Count > 1)
        {
            throw new RequestParameterValidationException("ServiceRequest", "ServiceRequest cannot be uniquely identified in the request bundle.");
        }
        return matching[0];
    }
}
