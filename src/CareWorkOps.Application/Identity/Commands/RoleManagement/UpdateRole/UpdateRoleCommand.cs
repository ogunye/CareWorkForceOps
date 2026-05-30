using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid TenantId,
    Guid RoleId,
    string RoleName)
    : IRequest<Result<RoleDto>>;