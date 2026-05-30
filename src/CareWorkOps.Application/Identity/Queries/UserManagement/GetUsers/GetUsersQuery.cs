using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.UserManagement.GetUsers;

public sealed record GetUsersQuery(
    Guid TenantId)
    : IRequest<Result<IReadOnlyCollection<UserDto>>>;