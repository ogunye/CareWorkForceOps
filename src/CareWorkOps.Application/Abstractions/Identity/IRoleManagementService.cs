using CareWorkOps.Application.Identity.Dtos;

namespace CareWorkOps.Application.Abstractions.Identity;

public interface IRoleManagementService
{
    Task<RoleDto?> CreateRoleAsync(
        Guid tenantId,
        string roleName,
        IReadOnlyCollection<string> permissions,
        CancellationToken cancellationToken = default);

    Task<RoleDto?> UpdateRoleAsync(
        Guid tenantId,
        Guid roleId,
        string roleName,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteRoleAsync(
        Guid tenantId,
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<RoleDto?> AssignPermissionsAsync(
        Guid tenantId,
        Guid roleId,
        IReadOnlyCollection<string> permissions,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(
        CancellationToken cancellationToken = default);
}