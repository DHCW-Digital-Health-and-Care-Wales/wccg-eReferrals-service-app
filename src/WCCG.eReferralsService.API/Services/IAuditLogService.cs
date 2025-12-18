using WCCG.eReferralsService.API.Constants;

namespace WCCG.eReferralsService.API.Services;

public interface IAuditLogService
{
    Task LogAsync(IHeaderDictionary headers, AuditEvents auditEvents);
}
