namespace CareWorkOps.Domain.Identity;

public static class PermissionCatalogue
{
    public static IReadOnlyCollection<Permission> All { get; } =
    [
        new(SystemPermissions.UsersView, "View Users", PermissionGroup.UserManagement, "View tenant users."),
        new(SystemPermissions.UsersCreate, "Create Users", PermissionGroup.UserManagement, "Create tenant users."),
        new(SystemPermissions.UsersUpdate, "Update Users", PermissionGroup.UserManagement, "Update tenant users."),
        new(SystemPermissions.UsersDeactivate, "Deactivate Users", PermissionGroup.UserManagement, "Deactivate or reactivate tenant users."),
        new(SystemPermissions.UsersAssignRoles, "Assign User Roles", PermissionGroup.UserManagement, "Assign or remove roles from tenant users."),

        new(SystemPermissions.RolesView, "View Roles", PermissionGroup.RoleManagement, "View tenant roles."),
        new(SystemPermissions.RolesCreate, "Create Roles", PermissionGroup.RoleManagement, "Create tenant roles."),
        new(SystemPermissions.RolesUpdate, "Update Roles", PermissionGroup.RoleManagement, "Update tenant roles."),
        new(SystemPermissions.RolesDelete, "Delete Roles", PermissionGroup.RoleManagement, "Delete tenant roles."),
        new(SystemPermissions.RolesAssignPermissions, "Assign Role Permissions", PermissionGroup.RoleManagement, "Assign permissions to tenant roles."),

        new(SystemPermissions.TenantsView, "View Tenant", PermissionGroup.TenantAdministration, "View tenant details."),
        new(SystemPermissions.TenantsUpdate, "Update Tenant", PermissionGroup.TenantAdministration, "Update tenant details."),
        new(SystemPermissions.TenantsActivate, "Activate Tenant", PermissionGroup.TenantAdministration, "Activate a tenant."),
        new(SystemPermissions.TenantsSuspend, "Suspend Tenant", PermissionGroup.TenantAdministration, "Suspend a tenant."),

        new(SystemPermissions.AuditView, "View Audit", PermissionGroup.Audit, "View audit trail."),
        new(SystemPermissions.ComplianceView, "View Compliance", PermissionGroup.Compliance, "View compliance dashboard.")
    ];

    public static bool IsValid(string permissionCode)
    {
        return All.Any(x => x.Code == permissionCode);
    }
}