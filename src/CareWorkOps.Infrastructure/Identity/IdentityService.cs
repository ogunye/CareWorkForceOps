using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace CareWorkOps.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
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
            new System.Security.Claims.Claim("TenantId", tenantId.ToString()));

        if (existingUsers.Any(user =>
                string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)))
        {
            return CreateUserResult.Failure([
                "A user with this email already exists for this tenant."
            ]);
        }

        var roleName = BuildTenantRoleName(tenantId, "TenantAdmin");

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await _roleManager.CreateAsync(
                new ApplicationRole(roleName, tenantId));

            if (!roleResult.Succeeded)
            {
                return CreateUserResult.Failure(
                    roleResult.Errors.Select(error => error.Description));
            }
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
            new System.Security.Claims.Claim("TenantId", tenantId.ToString()));

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

    private static string BuildTenantRoleName(Guid tenantId, string role)
    {
        return $"{tenantId:N}:{role}";
    }
}