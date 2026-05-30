using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.CreateRole;

public sealed class CreateRoleCommandHandler
    : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    private readonly IRoleManagementService _service;

    public CreateRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<Result<RoleDto>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await _service.CreateRoleAsync(
            request.TenantId,
            request.RoleName,
            request.Permissions,
            cancellationToken);

        return role is null
            ? Result<RoleDto>.Failure(Error.Failure("Unable to create role."))
            : Result<RoleDto>.Success(role);
    }
}