using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.RoleManagement.GetPermissions;

public sealed class GetPermissionsQueryHandler
    : IRequestHandler<GetPermissionsQuery, Result<IReadOnlyCollection<PermissionDto>>>
{
    private readonly IRoleManagementService _service;

    public GetPermissionsQueryHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<Result<IReadOnlyCollection<PermissionDto>>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await _service.GetPermissionsAsync(cancellationToken);

        return Result<IReadOnlyCollection<PermissionDto>>.Success(permissions);
    }
}