using CareWorkOps.Application.Identity.Dtos;

namespace CareWorkOps.Application.Abstractions.Identity;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserDto>> GetByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}