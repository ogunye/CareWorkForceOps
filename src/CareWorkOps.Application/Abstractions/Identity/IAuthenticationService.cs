using CareWorkOps.Application.Identity.Dtos;

namespace CareWorkOps.Application.Abstractions.Identity;

public interface IAuthenticationService
{
    Task<AuthenticatedUserDto?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthenticatedUserDto?> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<bool> RevokeRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<bool> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default);
}