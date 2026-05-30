using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Persistence.Auditing;
using CareWorkOps.Persistence.Context;

namespace CareWorkOps.Infrastructure.Auditing;

public sealed class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RecordAsync(
        Guid? tenantId,
        Guid? userId,
        string action,
        string entityName,
        string? entityId,
        string description,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(
            tenantId,
            userId,
            action,
            entityName,
            entityId,
            description);

        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}