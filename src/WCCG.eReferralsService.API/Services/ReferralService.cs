using System.Net.Http.Headers;
using System.Text.Json;
using FluentValidation;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Exceptions;
using WCCG.eReferralsService.API.Models;
using Task = System.Threading.Tasks.Task;

namespace WCCG.eReferralsService.API.Services;

public class ReferralService : IReferralService
{
    private readonly HttpClient _httpClient;
    private readonly IValidator<BundleModel> _bundleValidator;
    private readonly IValidator<HeadersModel> _headerValidator;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly PasReferralsApiConfig _pasReferralsApiConfig;

    public ReferralService(HttpClient httpClient,
        IOptions<PasReferralsApiConfig> pasReferralsApiOptions,
        IValidator<BundleModel> bundleValidator,
        IValidator<HeadersModel> headerValidator,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _bundleValidator = bundleValidator;
        _headerValidator = headerValidator;
        _jsonSerializerOptions = jsonSerializerOptions;
        _pasReferralsApiConfig = pasReferralsApiOptions.Value;
    }

    public async Task<string> CreateReferralAsync(IHeaderDictionary headers, string requestBody)
    {
        await ValidateHeadersAsync(headers);

        var bundle = JsonSerializer.Deserialize<Bundle>(requestBody, _jsonSerializerOptions);

        await ValidateBundleAsync(bundle!);

        //todo: Bundle trimming

        var response = await _httpClient.PostAsync(_pasReferralsApiConfig.CreateReferralEndpoint,
            new StringContent(requestBody, new MediaTypeHeaderValue(FhirConstants.FhirMediaType)));

        return await response.Content.ReadAsStringAsync();
    }

    private async Task ValidateHeadersAsync(IHeaderDictionary headers)
    {
        var headersModel = HeadersModel.FromHeaderDictionary(headers);

        var headersValidationResult = await _headerValidator.ValidateAsync(headersModel);
        if (!headersValidationResult.IsValid)
        {
            throw new HeaderValidationException(headersValidationResult.Errors);
        }
    }

    private async Task ValidateBundleAsync(Bundle bundle)
    {
        var bundleModel = BundleModel.FromBundle(bundle);

        var bundleValidationResult = await _bundleValidator.ValidateAsync(bundleModel);
        if (!bundleValidationResult.IsValid)
        {
            throw new BundleValidationException(bundleValidationResult.Errors);
        }
    }
}
