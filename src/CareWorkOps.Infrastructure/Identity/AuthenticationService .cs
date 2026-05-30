using System.Security.Cryptography;
using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Persistence.Context;
using CareWorkOps.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareWorkOps.Infrastructure.Identity;

public sealed class AuthenticationService : IAuthenticationService
{
    private const string PermissionClaimType = "Permission";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        ApplicationDbContext context,
        IAuditService auditService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _context = context;
        _auditService = auditService;
    }

    public async Task<AuthenticatedUserDto?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            await _auditService.RecordAsync(
                user.TenantId,
                user.Id,
                "LoginFailure",
                "ApplicationUser",
                user.Id.ToString(),
                $"Failed login attempt for user '{user.Email}'.",
                cancellationToken);

            return null;
        }

        await _auditService.RecordAsync(
            user.TenantId,
            user.Id,
            "LoginSuccess",
            "ApplicationUser",
            user.Id.ToString(),
            $"User '{user.Email}' logged in successfully.",
            cancellationToken);

        return await BuildAuthenticatedUserAsync(user, cancellationToken);
    }

    public async Task<AuthenticatedUserDto?> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return null;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == storedToken.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        storedToken.Revoke();

        await _auditService.RecordAsync(
            user.TenantId,
            user.Id,
            "RefreshTokenRotated",
            "RefreshToken",
            storedToken.Id.ToString(),
            $"Refresh token was rotated for user '{user.Email}'.",
            cancellationToken);

        return await BuildAuthenticatedUserAsync(user, cancellationToken);
    }

    public async Task<bool> RevokeRefreshTokenAsync(
        Guid userId,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Token == refreshToken,
                cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return false;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        storedToken.Revoke();

        await _context.SaveChangesAsync(cancellationToken);

        if (user is not null)
        {
            await _auditService.RecordAsync(
                user.TenantId,
                user.Id,
                "Logout",
                "RefreshToken",
                storedToken.Id.ToString(),
                $"User '{user.Email}' logged out.",
                cancellationToken);
        }

        return true;
    }

    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null || !user.IsActive)
        {
            return false;
        }

        var result = await _userManager.ChangePasswordAsync(
            user,
            currentPassword,
            newPassword);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            user.TenantId,
            user.Id,
            "PasswordChanged",
            "ApplicationUser",
            user.Id.ToString(),
            $"Password was changed for user '{user.Email}'.",
            cancellationToken);

        return true;
    }

    public async Task<bool> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return true;
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        if (string.IsNullOrWhiteSpace(resetToken))
        {
            return false;
        }

        await _auditService.RecordAsync(
            user.TenantId,
            user.Id,
            "PasswordResetRequested",
            "ApplicationUser",
            user.Id.ToString(),
            $"Password reset was requested for user '{user.Email}'.",
            cancellationToken);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            resetToken,
            newPassword);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            user.TenantId,
            user.Id,
            "PasswordResetCompleted",
            "ApplicationUser",
            user.Id.ToString(),
            $"Password reset was completed for user '{user.Email}'.",
            cancellationToken);

        return true;
    }

    private async Task<AuthenticatedUserDto> BuildAuthenticatedUserAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var displayRoles = roles
            .Select(RemoveTenantRolePrefix)
            .ToArray();

        var permissions = await GetUserPermissionsAsync(roles, cancellationToken);

        var tokenUser = new TokenUser(
            UserId: user.Id,
            TenantId: user.TenantId,
            Email: user.Email!,
            FullName: user.FullName,
            Roles: displayRoles,
            Permissions: permissions);

        var accessToken = _tokenService.CreateAccessToken(tokenUser);

        var refreshTokenValue = GenerateSecureRefreshToken();

        var newRefreshToken = new RefreshToken(
            user.Id,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(7));

        await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthenticatedUserDto(
            user.Id,
            user.TenantId,
            user.Email!,
            user.FullName,
            displayRoles,
            accessToken,
            refreshTokenValue);
    }

    private async Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(
        IEnumerable<string> roleNames,
        CancellationToken cancellationToken)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var roleName in roleNames)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);

            if (role is null)
            {
                continue;
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in claims.Where(x => x.Type == PermissionClaimType))
            {
                permissions.Add(claim.Value);
            }
        }

        return permissions.ToArray();
    }

    private static string GenerateSecureRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    private static string RemoveTenantRolePrefix(string roleName)
    {
        var separatorIndex = roleName.IndexOf(':');

        return separatorIndex < 0
            ? roleName
            : roleName[(separatorIndex + 1)..];
    }
}