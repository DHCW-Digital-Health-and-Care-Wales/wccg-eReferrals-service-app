namespace WCCG.eReferralsService.API.Configuration;

public class FhirValidationConfig
{
    public const string SectionName = "FhirValidation";

    public bool Enabled { get; set; } = true;

    public string[] PackagePaths { get; set; } = [];
}
