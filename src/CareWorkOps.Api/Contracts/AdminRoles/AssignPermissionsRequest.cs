namespace CareWorkOps.Api.Contracts.AdminRoles;

public sealed record AssignPermissionsRequest(
    IReadOnlyCollection<string> Permissions);