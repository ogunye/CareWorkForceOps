using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.CreateRole;

public sealed record CreateRoleCommand(
    Guid TenantId,
    string RoleName,
    IReadOnlyCollection<string> Permissions)
    : IRequest<Result<RoleDto>>;