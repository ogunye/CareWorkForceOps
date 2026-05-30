namespace CareWorkOps.Persistence.Auditing;

public sealed class AuditLog
{
    public Guid Id { get; private set; }

    public Guid? TenantId { get; private set; }

    public Guid? UserId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string EntityName { get; private set; } = string.Empty;

    public string? EntityId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    private AuditLog()
    {
    }

    public AuditLog(
        Guid? tenantId,
        Guid? userId,
        string action,
        string entityName,
        string? entityId,
        string description)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        UserId = userId;
        Action = action.Trim();
        EntityName = entityName.Trim();
        EntityId = entityId;
        Description = description.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }
}