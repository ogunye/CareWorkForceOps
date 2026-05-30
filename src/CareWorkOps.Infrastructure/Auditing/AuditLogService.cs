using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CareWorkOps.Infrastructure.Auditing;

public sealed class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;

    public AuditLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<AuditLogDto>> GetAuditLogsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new AuditLogDto(
                x.Id,
                x.TenantId,
                x.UserId,
                x.Action,
                x.EntityName,
                x.EntityId,
                x.Description,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<AuditLogDto?> GetAuditLogByIdAsync(
        Guid tenantId,
        Guid auditLogId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.Id == auditLogId)
            .Select(x => new AuditLogDto(
                x.Id,
                x.TenantId,
                x.UserId,
                x.Action,
                x.EntityName,
                x.EntityId,
                x.Description,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }
}