using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace NWRI.eReferralsService.API.Configuration;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers, Reason = "Used by configuration binder")]
public class FhirBundleProfileValidationConfig
{
    public const string SectionName = "FhirBundleProfileValidation";

    [Required]
    public bool Enabled { get; init; } = true;

    [Required]
    [Range(1, 100)]
    public int MaxConcurrentValidations { get; init; } = Environment.ProcessorCount;

    [Required]
    [Range(1, 300)]
    public int ValidationTimeoutSeconds { get; init; } = 10;
}
