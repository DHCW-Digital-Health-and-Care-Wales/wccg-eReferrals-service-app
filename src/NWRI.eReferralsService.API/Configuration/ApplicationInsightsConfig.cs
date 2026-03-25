using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NWRI.eReferralsService.API.Configuration;

[ExcludeFromCodeCoverage]
[UsedImplicitly(Reason = "Used by configuration binder")]
public class ApplicationInsightsConfig
{
    public static string SectionName => "ApplicationInsights";

    public required string ConnectionString { get; init; }
}
