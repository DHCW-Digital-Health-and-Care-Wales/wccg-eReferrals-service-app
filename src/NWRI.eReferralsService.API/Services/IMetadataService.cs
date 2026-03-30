namespace NWRI.eReferralsService.API.Services;
public interface IMetadataService
{
    Task<string> GetMetadataAsync(IHeaderDictionary headers, CancellationToken cancellationToken);
}
