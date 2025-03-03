using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using WCCG.eReferralsService.API.Configuration;
using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Services;

public class ReferralService : IReferralService
{
    private readonly HttpClient _httpClient;
    private readonly PasReferralsApiConfig _pasReferralsApiConfig;

    public ReferralService(HttpClient httpClient, IOptions<PasReferralsApiConfig> pasReferralsApiOptions)
    {
        _httpClient = httpClient;
        _pasReferralsApiConfig = pasReferralsApiOptions.Value;
    }

    public async Task<string> CreateReferralAsync(string bundleJson)
    {
        //todo: Bundle validation

        //todo: Bundle trimming

        var response = await _httpClient.PostAsync(_pasReferralsApiConfig.CreateReferralEndpoint,
            new StringContent(bundleJson, new MediaTypeHeaderValue(FhirConstants.FhirMediaType)));

        return await response.Content.ReadAsStringAsync();
    }
}
