using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.UpdateRole;

public sealed class UpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    private readonly IRoleManagementService _service;

    public UpdateRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<Result<RoleDto>> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await _service.UpdateRoleAsync(
            request.TenantId,
            request.RoleId,
            request.RoleName,
            cancellationToken);

        return role is null
            ? Result<RoleDto>.Failure(Error.Failure("Unable to update role."))
            : Result<RoleDto>.Success(role);
    }
}