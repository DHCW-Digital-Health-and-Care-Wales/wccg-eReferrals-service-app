namespace WCCG.eReferralsService.API.ApiClients;

public interface IPasReferralsApiClient
{
    Task<string> CreateReferralAsync(string bundleJson);
}
