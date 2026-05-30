using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.RoleManagement.GetPermissions;

public sealed record GetPermissionsQuery()
    : IRequest<Result<IReadOnlyCollection<PermissionDto>>>;