using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Helpers;

public static class FhirHttpErrorHelper
{
    private static readonly Dictionary<string, string> CodeDisplayDictionary = new()
    {
        { FhirHttpErrorCodes.ReceiverBadRequest, "400: The Receiver was unable to process the request." },
        { FhirHttpErrorCodes.ReceiverServerError, "500: The Receiver has encountered an error processing the request." }
    };

    public static string GetDisplayMessageByCode(string code)
    {
        return CodeDisplayDictionary.TryGetValue(code, out var displayMessage)
            ? displayMessage
            : string.Empty;
    }
}
