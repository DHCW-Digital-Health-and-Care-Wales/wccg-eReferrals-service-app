using System.Text.Json;

namespace WCCG.eReferralsService.API.Extensions;

public static class StringExtensions
{
    public static bool TryDeserializeJson<T>(this string inputString, out T? jsonObj) where T : class
    {
        try
        {
            jsonObj = JsonSerializer.Deserialize<T>(inputString);
            return true;
        }
        catch (JsonException)
        {
            jsonObj = null;
            return false;
        }
    }
}
