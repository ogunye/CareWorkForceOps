namespace CareWorkOps.Application.Auditing.Dtos;

public sealed record AuditLogDto(
    Guid Id,
    Guid? TenantId,
    Guid? UserId,
    string Action,
    string EntityName,
    string? EntityId,
    string Description,
    DateTime CreatedAtUtc);