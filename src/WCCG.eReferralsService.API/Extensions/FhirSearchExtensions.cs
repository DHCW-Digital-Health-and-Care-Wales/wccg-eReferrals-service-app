using Hl7.Fhir.Model;

namespace WCCG.eReferralsService.API.Extensions;

public static class FhirSearchExtensions
{
    public static T? ResourceByType<T>(this Bundle bundle, string? id = null) where T : Resource
    {
        var resultList = GetResourcesByType<T>(bundle).ToList();

        return id is not null
            ? resultList.FirstOrDefault(x => x!.Id.Equals(id, StringComparison.OrdinalIgnoreCase))
            : resultList.FirstOrDefault();
    }

    private static IEnumerable<T?> GetResourcesByType<T>(Bundle bundle) where T : Resource
    {
        return bundle.Children.OfType<Bundle.EntryComponent>()
            .Where(x => x.Resource is not null
                        && x.Resource.TypeName.Equals(typeof(T).Name, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Resource as T);
    }
}
