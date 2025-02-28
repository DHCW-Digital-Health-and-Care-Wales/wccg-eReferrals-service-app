using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WCCG.eReferralsService.API.Configuration;

[ExcludeFromCodeCoverage]
public class PasReferralsApiConfig
{
    public static string SectionName => "PasReferralsApi";

    [Required]
    public required string BaseUrl { get; set; }
}
