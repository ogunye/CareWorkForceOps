namespace CareWorkOps.Api.Contracts.AdminRoles;

public sealed record CreateRoleRequest(
    string RoleName,
    IReadOnlyCollection<string> Permissions);