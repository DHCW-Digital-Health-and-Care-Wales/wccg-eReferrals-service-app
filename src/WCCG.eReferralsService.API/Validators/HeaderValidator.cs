using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Exceptions;

namespace WCCG.eReferralsService.API.Validators;

public class HeaderValidator : IHeaderValidator
{
    private readonly IEnumerable<string> _requiredHeaders = RequestHeaderKeys.GetAllRequired();

    public void ValidateHeaders(IHeaderDictionary headerDictionary)
    {
        var missingHeaders = _requiredHeaders.Where(requiredHeader => !headerDictionary.ContainsKey(requiredHeader)).ToList();

        if (missingHeaders.Count != 0)
        {
            throw new MissingRequiredHeaderException(missingHeaders);
        }

        //todo: add headers format validation
    }
}
