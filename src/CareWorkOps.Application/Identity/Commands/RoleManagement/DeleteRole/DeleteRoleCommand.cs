using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.DeleteRole;

public sealed record DeleteRoleCommand(
    Guid TenantId,
    Guid RoleId)
    : IRequest<Result>;