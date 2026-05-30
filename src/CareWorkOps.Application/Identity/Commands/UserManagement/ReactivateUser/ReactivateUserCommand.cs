using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.ReactivateUser;

public sealed record ReactivateUserCommand(
    Guid TenantId,
    Guid UserId)
    : IRequest<Result>;