using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.RoleManagement.GetRoles;

public sealed record GetRolesQuery(
    Guid TenantId)
    : IRequest<Result<IReadOnlyCollection<RoleDto>>>;