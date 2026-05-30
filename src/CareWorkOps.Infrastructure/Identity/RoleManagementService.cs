using System.Security.Claims;
using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Domain.Identity;
using CareWorkOps.Persistence.Context;
using CareWorkOps.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareWorkOps.Infrastructure.Identity;

public sealed class RoleManagementService : IRoleManagementService
{
    private const string PermissionClaimType = "Permission";

    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public RoleManagementService(
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        IAuditService auditService)
    {
        _roleManager = roleManager;
        _context = context;
        _auditService = auditService;
    }

    public async Task<RoleDto?> CreateRoleAsync(
        Guid tenantId,
        string roleName,
        IReadOnlyCollection<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var fullRoleName = BuildTenantRoleName(tenantId, roleName);

        if (await _roleManager.RoleExistsAsync(fullRoleName))
        {
            return null;
        }

        var role = new ApplicationRole(fullRoleName, tenantId);

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            return null;
        }

        var permissionsUpdated = await ReplacePermissionsAsync(role, permissions);

        if (!permissionsUpdated)
        {
            return null;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "RoleCreated",
            "ApplicationRole",
            role.Id.ToString(),
            $"Role '{roleName}' was created.",
            cancellationToken);

        return await MapToDtoAsync(role);
    }

    public async Task<RoleDto?> UpdateRoleAsync(
        Guid tenantId,
        Guid roleId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await FindTenantRoleAsync(tenantId, roleId, cancellationToken);

        if (role is null)
        {
            return null;
        }

        var fullRoleName = BuildTenantRoleName(tenantId, roleName);

        role.Name = fullRoleName;
        role.NormalizedName = fullRoleName.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return null;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "RoleUpdated",
            "ApplicationRole",
            role.Id.ToString(),
            $"Role was renamed to '{roleName}'.",
            cancellationToken);

        return await MapToDtoAsync(role);
    }

    public async Task<bool> DeleteRoleAsync(
        Guid tenantId,
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var role = await FindTenantRoleAsync(tenantId, roleId, cancellationToken);

        if (role is null)
        {
            return false;
        }

        var displayRoleName = RemoveTenantRolePrefix(role.Name ?? string.Empty);

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return false;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "RoleDeleted",
            "ApplicationRole",
            role.Id.ToString(),
            $"Role '{displayRoleName}' was deleted.",
            cancellationToken);

        return true;
    }

    public async Task<RoleDto?> AssignPermissionsAsync(
        Guid tenantId,
        Guid roleId,
        IReadOnlyCollection<string> permissions,
        CancellationToken cancellationToken = default)
    {
        var role = await FindTenantRoleAsync(tenantId, roleId, cancellationToken);

        if (role is null)
        {
            return null;
        }

        var updated = await ReplacePermissionsAsync(role, permissions);

        if (!updated)
        {
            return null;
        }

        await _auditService.RecordAsync(
            tenantId,
            null,
            "RolePermissionsAssigned",
            "ApplicationRole",
            role.Id.ToString(),
            $"Permissions were updated for role '{RemoveTenantRolePrefix(role.Name ?? string.Empty)}'.",
            cancellationToken);

        return await MapToDtoAsync(role);
    }

    public async Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = new List<RoleDto>();

        foreach (var role in roles)
        {
            result.Add(await MapToDtoAsync(role));
        }

        return result;
    }

    public Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PermissionDto> permissions = PermissionCatalogue.All
            .Select(permission => new PermissionDto(
                permission.Code,
                permission.Name,
                permission.Group,
                permission.Description))
            .ToArray();

        return Task.FromResult(permissions);
    }

    private async Task<ApplicationRole?> FindTenantRoleAsync(
        Guid tenantId,
        Guid roleId,
        CancellationToken cancellationToken)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(
                x => x.Id == roleId &&
                     x.TenantId == tenantId,
                cancellationToken);
    }

    private async Task<bool> ReplacePermissionsAsync(
        ApplicationRole role,
        IReadOnlyCollection<string> permissions)
    {
        var existingClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in existingClaims.Where(x => x.Type == PermissionClaimType))
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);

            if (!removeResult.Succeeded)
            {
                return false;
            }
        }

        foreach (var permission in permissions.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!PermissionCatalogue.IsValid(permission))
            {
                return false;
            }

            var addResult = await _roleManager.AddClaimAsync(
                role,
                new Claim(PermissionClaimType, permission));

            if (!addResult.Succeeded)
            {
                return false;
            }
        }

        return true;
    }

    private async Task<RoleDto> MapToDtoAsync(ApplicationRole role)
    {
        var claims = await _roleManager.GetClaimsAsync(role);

        var permissions = claims
            .Where(x => x.Type == PermissionClaimType)
            .Select(x => x.Value)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        return new RoleDto(
            Id: role.Id,
            TenantId: role.TenantId,
            Name: RemoveTenantRolePrefix(role.Name ?? string.Empty),
            Permissions: permissions);
    }

    private static string BuildTenantRoleName(Guid tenantId, string role)
    {
        return $"{tenantId:N}:{role.Trim()}";
    }

    private static string RemoveTenantRolePrefix(string roleName)
    {
        var index = roleName.IndexOf(':');

        return index < 0
            ? roleName
            : roleName[(index + 1)..];
    }
}