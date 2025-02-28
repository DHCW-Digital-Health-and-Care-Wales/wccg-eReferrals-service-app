namespace WCCG.eReferralsService.API.Services;

public interface IReferralService
{
    Task<string> CreateReferralAsync(string bundleJson);
}
