using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NWRI.eReferralsService.API.Configuration;

[ExcludeFromCodeCoverage]
[UsedImplicitly(Reason = "Used by configuration binder")]
public class ManagedIdentityConfig
{
    public static string SectionName => "ManagedIdentity";

    public required string ClientId { get; init; }
}
