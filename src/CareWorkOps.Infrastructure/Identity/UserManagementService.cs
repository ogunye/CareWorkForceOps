using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Persistence.Context;
using CareWorkOps.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareWorkOps.Infrastructure.Identity;

public sealed class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        IAuditService auditService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _auditService = auditService;
    }

    public async Task<UserDto?> CreateUserAsync(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string password,
        IReadOnlyCollection<string> roles,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();

        var existingUser = await _context.Users.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (existingUser)
        {
            return null;
        }

        var user = new ApplicationUser(
            tenantId,
            firstName,
            lastName,
            email);

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return null;
        }

        foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var roleName = BuildTenantRoleName(tenantId, role);

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(
                    new ApplicationRole(roleName, tenantId));

                if (!roleResult.Succeeded)
                {
                    return null;
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!addRoleResult.Succeeded)
            {
                return null;
            }
        }

        var claimResult = await _userManager.AddClaimAsync(
            user,
            new System.Security.Claims.Claim("TenantId", tenantId.ToString()));

        if (!claimResult.Succeeded)
        {
            return null;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "UserCreated",
            "ApplicationUser",
            user.Id.ToString(),
            $"User '{user.Email}' was created.",
            cancellationToken);

        return await MapToDtoAsync(user);
    }

    public async Task<UserDto?> UpdateUserAsync(
        Guid tenantId,
        Guid userId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var user = await FindTenantUserAsync(tenantId, userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.UpdateProfile(firstName, lastName);

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return null;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "UserUpdated",
            "ApplicationUser",
            user.Id.ToString(),
            $"User '{user.Email}' profile was updated.",
            cancellationToken);

        return await MapToDtoAsync(user);
    }

    public async Task<bool> DeactivateUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindTenantUserAsync(tenantId, userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Deactivate();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "UserDeactivated",
            "ApplicationUser",
            user.Id.ToString(),
            $"User '{user.Email}' was deactivated.",
            cancellationToken);

        return true;
    }

    public async Task<bool> ReactivateUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindTenantUserAsync(tenantId, userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Reactivate();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "UserReactivated",
            "ApplicationUser",
            user.Id.ToString(),
            $"User '{user.Email}' was reactivated.",
            cancellationToken);

        return true;
    }

    public async Task<bool> AssignRoleAsync(
        Guid tenantId,
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var user = await FindTenantUserAsync(tenantId, userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        var roleName = BuildTenantRoleName(tenantId, role);

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await _roleManager.CreateAsync(
                new ApplicationRole(roleName, tenantId));

            if (!roleResult.Succeeded)
            {
                return false;
            }
        }

        if (await _userManager.IsInRoleAsync(user, roleName))
        {
            return true;
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "UserRoleAssigned",
            "ApplicationUser",
            user.Id.ToString(),
            $"Role '{role}' was assigned to user '{user.Email}'.",
            cancellationToken);

        return true;
    }

    public async Task<bool> RemoveRoleAsync(
        Guid tenantId,
        Guid userId,
        string role,
        CancellationToken cancellationToken = default)
    {
        var user = await FindTenantUserAsync(tenantId, userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        var roleName = BuildTenantRoleName(tenantId, role);

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            return true;
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "UserRoleRemoved",
            "ApplicationUser",
            user.Id.ToString(),
            $"Role '{role}' was removed from user '{user.Email}'.",
            cancellationToken);

        return true;
    }

    public async Task<IReadOnlyCollection<UserDto>> GetUsersAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync(cancellationToken);

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            result.Add(await MapToDtoAsync(user));
        }

        return result;
    }

    public async Task<UserDto?> GetUserByIdAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindTenantUserAsync(
            tenantId,
            userId,
            cancellationToken);

        return user is null
            ? null
            : await MapToDtoAsync(user);
    }

    private async Task<ApplicationUser?> FindTenantUserAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == userId &&
                     x.TenantId == tenantId,
                cancellationToken);
    }

    private async Task<UserDto> MapToDtoAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var displayRoles = roles
            .Select(RemoveTenantRolePrefix)
            .ToArray();

        return new UserDto(
            Id: user.Id,
            TenantId: user.TenantId,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email!,
            Status: user.Status,
            Roles: displayRoles);
    }

    private static string BuildTenantRoleName(Guid tenantId, string role)
    {
        return $"{tenantId:N}:{role.Trim()}";
    }

    private static string RemoveTenantRolePrefix(string roleName)
    {
        var separatorIndex = roleName.IndexOf(':');

        return separatorIndex < 0
            ? roleName
            : roleName[(separatorIndex + 1)..];
    }
}