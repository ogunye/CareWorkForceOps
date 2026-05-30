using CareWorkOps.Application.Identity.Dtos;

namespace CareWorkOps.Application.Abstractions.Identity;

public interface IRoleRepository
{
    Task<bool> RoleExistsAsync(
        Guid tenantId,
        string roleName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}