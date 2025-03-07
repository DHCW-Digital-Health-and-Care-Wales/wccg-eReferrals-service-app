namespace WCCG.eReferralsService.API.Services;

public interface IReferralService
{
    Task<string> CreateReferralAsync(IHeaderDictionary headers, string requestBody);
}
