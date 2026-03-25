using Hl7.Fhir.Utility;
using NWRI.eReferralsService.API.Constants;

namespace NWRI.eReferralsService.API.Models;

public class HeadersModel
{
    //Required
    public required string? TargetIdentifier { get; init; }
    public required string? EndUserOrganisation { get; init; }
    public required string? RequestingSoftware { get; init; }
    public required string? RequestId { get; init; }
    public required string? CorrelationId { get; init; }
    public required string? UseContext { get; init; }
    public required string? Accept { get; init; }

    //Optional
    public required string? RequestingPractitioner { get; init; }

    public static HeadersModel FromHeaderDictionary(IHeaderDictionary headerDictionary)
    {
        return new HeadersModel
        {
            //Required
            TargetIdentifier = headerDictionary.GetOrDefault(RequestHeaderKeys.TargetIdentifier),
            EndUserOrganisation = headerDictionary.GetOrDefault(RequestHeaderKeys.EndUserOrganisation),
            RequestingSoftware = headerDictionary.GetOrDefault(RequestHeaderKeys.RequestingSoftware),
            RequestId = headerDictionary.GetOrDefault(RequestHeaderKeys.RequestId),
            CorrelationId = headerDictionary.GetOrDefault(RequestHeaderKeys.CorrelationId),
            UseContext = headerDictionary.GetOrDefault(RequestHeaderKeys.UseContext),
            Accept = headerDictionary.GetOrDefault(RequestHeaderKeys.Accept),

            //Optional
            RequestingPractitioner = headerDictionary.GetOrDefault(RequestHeaderKeys.RequestingPractitioner)
        };
    }
}
