using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.AssignPermissionsToRole;

public sealed class AssignPermissionsToRoleCommandHandler
    : IRequestHandler<AssignPermissionsToRoleCommand, Result<RoleDto>>
{
    private readonly IRoleManagementService _service;

    public AssignPermissionsToRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<Result<RoleDto>> Handle(
        AssignPermissionsToRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await _service.AssignPermissionsAsync(
            request.TenantId,
            request.RoleId,
            request.Permissions,
            cancellationToken);

        return role is null
            ? Result<RoleDto>.Failure(Error.Failure("Unable to assign permissions."))
            : Result<RoleDto>.Success(role);
    }
}