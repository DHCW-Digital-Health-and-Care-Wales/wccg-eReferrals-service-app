using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using NWRI.eReferralsService.API.Configuration.Resilience;

namespace NWRI.eReferralsService.API.Configuration.OptionValidators;

[OptionsValidator]
[UsedImplicitly(Reason = "Used by configuration binder")]
public partial class ValidateRetryConfigOptions : IValidateOptions<RetryConfig>;
