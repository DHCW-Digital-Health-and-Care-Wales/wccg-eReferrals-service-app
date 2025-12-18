using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Services;

public partial class AuditLogService : IAuditLogService
{
    private static partial class Log
    {
        [LoggerMessage(
            Level = LogLevel.Information,
            Message = "AuditLog AuditEvent={AuditEvent} TimestampUtc={TimestampUtc} X-Request-Id={RequestId} X-Correlation-Id={CorrelationId} EndUserOrganisation={EndUserOrganisation} RequestingSoftware={RequestingSoftware}")]
        public static partial void AuditLog(
            ILogger logger,
            AuditEvents auditEvent,
            DateTimeOffset timestampUtc,
            string requestId,
            string correlationId,
            string endUserOrganisation,
            string requestingSoftware);
    }

    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(ILogger<AuditLogService> logger)
    {
        _logger = logger;
    }

    public Task LogAsync(IHeaderDictionary headers, AuditEvents auditEvents)
    {
        var timestampUtc = DateTimeOffset.UtcNow;

        headers.TryGetValue(RequestHeaderKeys.RequestId, out var requestId);
        headers.TryGetValue(RequestHeaderKeys.CorrelationId, out var correlationId);
        headers.TryGetValue(RequestHeaderKeys.EndUserOrganisation, out var endUserOrganisation);
        headers.TryGetValue(RequestHeaderKeys.RequestingSoftware, out var requestingSoftware);

        Log.AuditLog(
            _logger,
            auditEvents,
            timestampUtc,
            requestId.ToString(),
            correlationId.ToString(),
            endUserOrganisation.ToString(),
            requestingSoftware.ToString());

        return Task.CompletedTask;
    }
}
