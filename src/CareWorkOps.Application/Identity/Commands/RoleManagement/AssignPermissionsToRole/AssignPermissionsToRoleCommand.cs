using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.AssignPermissionsToRole;

public sealed record AssignPermissionsToRoleCommand(
    Guid TenantId,
    Guid RoleId,
    IReadOnlyCollection<string> Permissions)
    : IRequest<Result<RoleDto>>;