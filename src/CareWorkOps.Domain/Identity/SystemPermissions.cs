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

    public const string AdminUsersView = "AdminUsers.View";
    public const string AdminUsersCreate = "AdminUsers.Create";
    public const string AdminUsersUpdate = "AdminUsers.Update";
    public const string AdminUsersDeactivate = "AdminUsers.Deactivate";

    public const string AdminRolesView = "AdminRoles.View";
    public const string AdminRolesCreate = "AdminRoles.Create";
    public const string AdminRolesUpdate = "AdminRoles.Update";

    public static readonly string[] All =
    [
        AuditView,
        AdminUsersView,
        AdminUsersCreate,
        AdminUsersUpdate,
        AdminUsersDeactivate,
        AdminRolesView,
        AdminRolesCreate,
        AdminRolesUpdate,
        TenantsActivate,
        TenantsSuspend,
        TenantsUpdate,
        TenantsView,
        UsersAssignRoles,
        UsersCreate,
        UsersDeactivate,
        UsersUpdate,
        UsersView,
        RolesAssignPermissions,
        RolesCreate,
        RolesDelete,
        RolesUpdate,
        RolesView
    ];
}