using System.Security.Claims;
using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Domain.Identity;
using CareWorkOps.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace CareWorkOps.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private const string PermissionClaimType = "Permission";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<CreateUserResult> CreateTenantAdminAsync(
        Guid tenantId,
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var existingUsers = await _userManager.GetUsersForClaimAsync(
            new Claim("TenantId", tenantId.ToString()));

        if (existingUsers.Any(user =>
                string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)))
        {
            return CreateUserResult.Failure([
                "A user with this email already exists for this tenant."
            ]);
        }

        var roleName = BuildTenantRoleName(tenantId, SystemRoles.TenantAdmin);

        var role = await EnsureTenantAdminRoleAsync(tenantId, roleName);

        if (role is null)
        {
            return CreateUserResult.Failure([
                "Unable to create TenantAdmin role."
            ]);
        }

        var user = new ApplicationUser(
            tenantId,
            firstName,
            lastName,
            email);

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return CreateUserResult.Failure(
                createResult.Errors.Select(error => error.Description));
        }

        var claimResult = await _userManager.AddClaimAsync(
            user,
            new Claim("TenantId", tenantId.ToString()));

        if (!claimResult.Succeeded)
        {
            return CreateUserResult.Failure(
                claimResult.Errors.Select(error => error.Description));
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

        if (!addRoleResult.Succeeded)
        {
            return CreateUserResult.Failure(
                addRoleResult.Errors.Select(error => error.Description));
        }

        return CreateUserResult.Success(user.Id);
    }

    private async Task<ApplicationRole?> EnsureTenantAdminRoleAsync(
        Guid tenantId,
        string roleName)
    {
        ApplicationRole? role;

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            role = new ApplicationRole(roleName, tenantId);

            var roleResult = await _roleManager.CreateAsync(role);

            if (!roleResult.Succeeded)
            {
                return null;
            }
        }
        else
        {
            role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                return null;
            }
        }

        await SeedTenantAdminPermissionsAsync(role);

        return role;
    }

    private async Task SeedTenantAdminPermissionsAsync(ApplicationRole role)
    {
        var existingClaims = await _roleManager.GetClaimsAsync(role);

        var existingPermissions = existingClaims
            .Where(claim => claim.Type == PermissionClaimType)
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var permission in PermissionCatalogue.All)
        {
            if (existingPermissions.Contains(permission.Code))
            {
                continue;
            }

            await _roleManager.AddClaimAsync(
                role,
                new Claim(PermissionClaimType, permission.Code));
        }
    }

    private static string BuildTenantRoleName(Guid tenantId, string role)
    {
        return $"{tenantId:N}:{role}";
    }
}