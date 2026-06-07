using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;

namespace CareWorkOps.Application.Abstractions.Identity;

public interface IUserManagementService
{
    Task<Result<UserDto>> CreateUserAsync(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string password,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default);

    Task<UserDto?> UpdateUserAsync(
        Guid tenantId,
        Guid userId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> ReactivateUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> AssignRoleAsync(
        Guid tenantId,
        Guid userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveRoleAsync(
        Guid tenantId,
        Guid userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UserDto>> GetUsersAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetUserByIdAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default);
}