using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.CreateUser;

public sealed record CreateUserCommand(
    Guid TenantId,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IReadOnlyCollection<string> Roles)
    : IRequest<Result<UserDto>>;