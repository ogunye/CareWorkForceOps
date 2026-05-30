namespace CareWorkOps.Domain.Identity;

public static class SystemPermissions
{
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDeactivate = "users.deactivate";
    public const string UsersAssignRoles = "users.assign_roles";

    public const string RolesView = "roles.view";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";
    public const string RolesAssignPermissions = "roles.assign_permissions";

    public const string TenantsView = "tenants.view";
    public const string TenantsUpdate = "tenants.update";
    public const string TenantsActivate = "tenants.activate";
    public const string TenantsSuspend = "tenants.suspend";

    public const string AuditView = "audit.view";
    public const string ComplianceView = "compliance.view";
}