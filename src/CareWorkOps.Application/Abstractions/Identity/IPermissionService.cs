using CareWorkOps.Application.Identity.Dtos;

namespace CareWorkOps.Application.Abstractions.Identity;

public interface IPermissionService
{
    Task<IReadOnlyCollection<PermissionDto>> GetAllPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<bool> UserHasPermissionAsync(
        Guid tenantId,
        Guid userId,
        string permissionCode,
        CancellationToken cancellationToken = default);
}