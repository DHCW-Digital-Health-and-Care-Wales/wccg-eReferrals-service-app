using System.Diagnostics.CodeAnalysis;
using WCCG.eReferralsService.API.Exceptions;

namespace WCCG.eReferralsService.API.Extensions;

[ExcludeFromCodeCoverage]
public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Called {methodName}.")]
    public static partial void CalledMethod(this ILogger logger, string methodName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Required header(s) missing.")]
    public static partial void RequiredHeadersMissingError(this ILogger logger, MissingRequiredHeaderException exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unexpected error.")]
    public static partial void UnexpectedError(this ILogger logger, Exception exception);
}
