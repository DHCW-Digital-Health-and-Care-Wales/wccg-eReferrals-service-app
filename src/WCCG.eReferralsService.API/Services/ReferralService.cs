using WCCG.eReferralsService.API.ApiClients;

namespace WCCG.eReferralsService.API.Services;

public class ReferralService : IReferralService
{
    private readonly IPasReferralsApiClient _apiClient;

    public ReferralService(IPasReferralsApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<string> CreateReferralAsync(string bundleJson)
    {
        //todo: Bundle validation

        //todo: Bundle trimming

        return await _apiClient.CreateReferralAsync(bundleJson);
    }
}
