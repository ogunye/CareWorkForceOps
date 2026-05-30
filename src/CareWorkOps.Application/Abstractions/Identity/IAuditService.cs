namespace CareWorkOps.Application.Abstractions.Auditing;

public interface IAuditService
{
    Task RecordAsync(
        Guid? tenantId,
        Guid? userId,
        string action,
        string entityName,
        string? entityId,
        string description,
        CancellationToken cancellationToken = default);
}