using FluentValidation;
using NWRI.eReferralsService.API.Models;

namespace NWRI.eReferralsService.API.Validators;

public interface IMetadataHeadersValidator : IValidator<HeadersModel>
{
}
