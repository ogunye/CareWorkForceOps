using CareWorkOps.Application.Auditing.Dtos;

namespace CareWorkOps.Application.Abstractions.Auditing;

public interface IAuditLogService
{
    Task<IReadOnlyCollection<AuditLogDto>> GetAuditLogsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<AuditLogDto?> GetAuditLogByIdAsync(
        Guid tenantId,
        Guid auditLogId,
        CancellationToken cancellationToken = default);
}