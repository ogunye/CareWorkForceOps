using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.UserManagement.GetUserById;

public sealed record GetUserByIdQuery(
    Guid TenantId,
    Guid UserId)
    : IRequest<Result<UserDto>>;