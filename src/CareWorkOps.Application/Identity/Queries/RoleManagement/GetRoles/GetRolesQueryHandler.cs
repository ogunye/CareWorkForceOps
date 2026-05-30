using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Queries.RoleManagement.GetRoles;

public sealed class GetRolesQueryHandler
    : IRequestHandler<GetRolesQuery, Result<IReadOnlyCollection<RoleDto>>>
{
    private readonly IRoleManagementService _service;

    public GetRolesQueryHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<Result<IReadOnlyCollection<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await _service.GetRolesAsync(
            request.TenantId,
            cancellationToken);

        return Result<IReadOnlyCollection<RoleDto>>.Success(roles);
    }
}