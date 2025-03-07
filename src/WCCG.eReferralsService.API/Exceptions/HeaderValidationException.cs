using FluentValidation.Results;
using WCCG.eReferralsService.API.Constants;
using WCCG.eReferralsService.API.Errors;

namespace WCCG.eReferralsService.API.Exceptions;

public class HeaderValidationException : BaseFhirException
{
    private readonly Dictionary<string, Func<string, BaseHeaderError>> _errorTypeDictionary = new()
    {
        { ValidationErrorCodes.MissingRequiredHeaderCode, message => new MissingRequiredHeaderError(message) },
        { ValidationErrorCodes.InvalidHeaderCode, message => new InvalidHeaderError(message) }
    };

    private readonly IEnumerable<ValidationFailure> _validationErrors;

    public HeaderValidationException(IEnumerable<ValidationFailure> validationErrors)
    {
        _validationErrors = validationErrors;
    }

    public override IEnumerable<BaseFhirHttpError> Errors =>
        _validationErrors.Select(error => _errorTypeDictionary[error.ErrorCode](error.ErrorMessage));

    public override string Message => $"Header(s) validation failure: {string.Join(';', _validationErrors.Select(x => x.ErrorMessage))}";
}
