using System.Net.Http.Headers;
using WCCG.eReferralsService.API.ApiClients.Endpoints;
using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.ApiClients;

public class PasReferralsApiClient : IPasReferralsApiClient
{
    private readonly HttpClient _httpClient;

    public PasReferralsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateReferralAsync(string bundleJson)
    {
        var response = await _httpClient.PostAsync(PasReferralsApiEndpoints.CreateReferralEndpoint,
            new StringContent(bundleJson, new MediaTypeHeaderValue(FhirConstants.FhirMediaType)));

        return await response.Content.ReadAsStringAsync();
    }
}
