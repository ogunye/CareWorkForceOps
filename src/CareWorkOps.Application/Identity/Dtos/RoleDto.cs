namespace CareWorkOps.Application.Identity.Dtos
{
    public sealed record RoleDto(
     Guid Id,
     Guid? TenantId,
     string Name,
     IReadOnlyCollection<string> Permissions);
}
