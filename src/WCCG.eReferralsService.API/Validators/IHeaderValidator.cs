namespace WCCG.eReferralsService.API.Validators;

public interface IHeaderValidator
{
    void ValidateHeaders(IHeaderDictionary headerDictionary);
}
