using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.AssignRole;

public sealed record AssignRoleCommand(
    Guid TenantId,
    Guid UserId,
    string Role)
    : IRequest<Result>;