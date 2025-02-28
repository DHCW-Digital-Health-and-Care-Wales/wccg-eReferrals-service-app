namespace WCCG.eReferralsService.API.Constants;

public static class RequestHeaderKeys
{
    //Required
    public const string TargetIdentifier = "NHSD-Target-Identifier";
    public const string EndUserOrganisation = "NHSD-End-User-Organisation";
    public const string RequestingSoftware = "NHSD-Requesting-Software";
    public const string RequestId = "X-Request-Id";
    public const string CorrelationId = "X-Correlation-Id";
    public const string UseContext = "use-context";
    public const string Accept = "Accept";

    //Optional
    private const string RequestingPractitioner = "NHSD-Requesting-Practitioner";

    public static IEnumerable<string> GetAllRequired()
    {
        return [TargetIdentifier, EndUserOrganisation, RequestingSoftware, RequestId, CorrelationId, UseContext, Accept];
    }

    public static IEnumerable<string> GetAllOptional()
    {
        return [RequestingPractitioner];
    }
}
