using JetBrains.Annotations;

namespace NWRI.eReferralsService.API.Models.WPAS.Responses;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers, Reason = "Part of API contract")]
public record WpasCreateReferralResponse : WpasReferralResponse
{
    public required string System { get; init; }
    public required string AssigningAuthority { get; init; }
    public required string OrganisationCode { get; init; }
    public required string OrganisationName { get; init; }
    public required string ReferralCreationTimestamp { get; init; }
}
