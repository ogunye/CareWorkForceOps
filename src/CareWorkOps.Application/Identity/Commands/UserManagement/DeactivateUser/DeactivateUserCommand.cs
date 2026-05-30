using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.DeactivateUser;

public sealed record DeactivateUserCommand(
    Guid TenantId,
    Guid UserId)
    : IRequest<Result>;