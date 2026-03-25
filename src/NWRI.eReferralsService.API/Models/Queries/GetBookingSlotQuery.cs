using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace NWRI.eReferralsService.API.Models.Queries;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers, Reason = "Used by framework itself")]
public class GetBookingSlotQuery
{
    [FromQuery(Name = "status")]
    public required string Status { get; set; }
    [FromQuery(Name = "start")]
    public required string[] Start { get; set; }
    [FromQuery(Name = "_include")]
    public required string[] Include { get; set; }
}
