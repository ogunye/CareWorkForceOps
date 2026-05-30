using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.RemoveRole;

public sealed record RemoveRoleCommand(
    Guid TenantId,
    Guid UserId,
    string Role)
    : IRequest<Result>;