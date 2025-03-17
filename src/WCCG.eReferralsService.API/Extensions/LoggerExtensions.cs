using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Hl7.Fhir.Serialization;
using WCCG.eReferralsService.API.Exceptions;

namespace WCCG.eReferralsService.API.Extensions;

[ExcludeFromCodeCoverage]
public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Called {methodName}.")]
    public static partial void CalledMethod(this ILogger logger, string methodName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Header(s) validation error.")]
    public static partial void HeadersValidationError(this ILogger logger, HeaderValidationException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Bundle validation error.")]
    public static partial void BundleValidationError(this ILogger logger, BundleValidationException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to deserialize bundle.")]
    public static partial void BundleDeserializationFailure(this ILogger logger, DeserializationFailedException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Invalid JSON.")]
    public static partial void InvalidJson(this ILogger logger, JsonException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "An error occurred while calling the PAS API.")]
    public static partial void ApiCallError(this ILogger logger, HttpRequestException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "PAS API returned not successful response.")]
    public static partial void NotSuccessfulApiResponseError(this ILogger logger, NotSuccessfulApiCallException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unexpected error.")]
    public static partial void UnexpectedError(this ILogger logger, Exception exception);
}
