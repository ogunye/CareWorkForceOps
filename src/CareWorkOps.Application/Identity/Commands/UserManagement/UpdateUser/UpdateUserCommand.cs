using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.UpdateUser;

public sealed record UpdateUserCommand(
    Guid TenantId,
    Guid UserId,
    string FirstName,
    string LastName)
    : IRequest<Result<UserDto>>;