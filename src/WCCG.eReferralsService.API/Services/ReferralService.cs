using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using FluentValidation;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Models;
using WCCG.eReferralsService.API.Validators;
using Task = System.Threading.Tasks.Task;

namespace WCCG.eReferralsService.API.Services;

public class ReferralService : IReferralService
{
    private readonly HttpClient _httpClient;
    private readonly IValidator<BundleModel> _bundleValidator;
    private readonly IFhirBundleProfileValidator _fhirBundleProfileValidator;
    private readonly IValidator<HeadersModel> _headerValidator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly PasReferralsApiConfig _pasReferralsApiConfig;
    private readonly IAuditLogService _auditLogService;

    public ReferralService(HttpClient httpClient,
        IOptions<PasReferralsApiConfig> pasReferralsApiOptions,
        IValidator<BundleModel> bundleValidator,
        IFhirBundleProfileValidator fhirBundleProfileValidator,
        IValidator<HeadersModel> headerValidator,
        IAuditLogService auditLogService,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _bundleValidator = bundleValidator;
        _fhirBundleProfileValidator = fhirBundleProfileValidator;
        _headerValidator = headerValidator;
        _auditLogService = auditLogService;
        _jsonSerializerOptions = jsonSerializerOptions;
        _pasReferralsApiConfig = pasReferralsApiOptions.Value;
    }

    public async Task<string> CreateReferralAsync(IHeaderDictionary headers, string requestBody)
    {
        await ValidateHeaders(headers);

        var bundle = JsonSerializer.Deserialize<Bundle>(requestBody, _jsonSerializerOptions);
        await ValidateFhirProfile(headers, bundle!);
        await ValidateMandatoryData(headers, bundle!);

        using var response = await _httpClient.PostAsync(_pasReferralsApiConfig.CreateReferralEndpoint,
            new StringContent(requestBody, new MediaTypeHeaderValue(FhirConstants.FhirMediaType)));

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw await GetNotSuccessfulApiCallException(response);
    }

    public async Task<string> GetReferralAsync(IHeaderDictionary headers, string? id)
    {
        if (!Guid.TryParse(id, out _))
        {
            throw new RequestParameterValidationException(nameof(id), "Id should be a valid GUID");
        }

        await ValidateHeaders(headers);

        var endpoint = string.Format(CultureInfo.InvariantCulture, _pasReferralsApiConfig.GetReferralEndpoint, id);
        using var response = await _httpClient.GetAsync(endpoint);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw await GetNotSuccessfulApiCallException(response);
    }

    private static async Task<Exception> GetNotSuccessfulApiCallException(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content);
            return new NotSuccessfulApiCallException(response.StatusCode, problemDetails!);
        }
        catch (JsonException)
        {
            return new NotSuccessfulApiCallException(response.StatusCode, content);
        }
    }

    private async Task ValidateHeaders(IHeaderDictionary headers)
    {
        try
        {
            var headersModel = HeadersModel.FromHeaderDictionary(headers);

            var headersValidationResult = await _headerValidator.ValidateAsync(headersModel);
            if (!headersValidationResult.IsValid)
            {
                throw new HeaderValidationException(headersValidationResult.Errors);
            }

            await _auditLogService.LogAsync(headers, AuditEvents.HeadersValidationSucceeded);
        }
        catch (HeaderValidationException)
        {
            await _auditLogService.LogAsync(headers, AuditEvents.HeadersValidationFailed);
            throw;
        }
    }

    private async Task ValidateFhirProfile(IHeaderDictionary headers, Bundle bundle)
    {
        var profileOutcome = _fhirBundleProfileValidator.Validate(bundle);
        if (!IsSuccessful(profileOutcome))
        {
            await _auditLogService.LogAsync(headers, AuditEvents.FhirProfileValidationFailed);
            throw new FhirProfileValidationException(profileOutcome);
        }

        await _auditLogService.LogAsync(headers, AuditEvents.FhirProfileValidationSucceeded);
    }

    private async Task ValidateMandatoryData(IHeaderDictionary headers, Bundle bundle)
    {
        try
        {
            var bundleModel = BundleModel.FromBundle(bundle);

            var bundleValidationResult = await _bundleValidator.ValidateAsync(bundleModel);
            if (!bundleValidationResult.IsValid)
            {
                throw new BundleValidationException(bundleValidationResult.Errors);
            }

            await _auditLogService.LogAsync(headers, AuditEvents.MandatoryDataValidationSucceeded);
        }
        catch (BundleValidationException)
        {
            await _auditLogService.LogAsync(headers, AuditEvents.MandatoryDataValidationFailed);
            throw;
        }
    }

    private static bool IsSuccessful(OperationOutcome outcome)
    {
        return !outcome.Issue.Any(i => i.Severity is OperationOutcome.IssueSeverity.Error or OperationOutcome.IssueSeverity.Fatal);
    }
}
