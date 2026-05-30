using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.DeleteRole;

public sealed class DeleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly IRoleManagementService _service;

    public DeleteRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<Result> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _service.DeleteRoleAsync(
            request.TenantId,
            request.RoleId,
            cancellationToken);

        return success
            ? Result.Success()
            : Result.Failure(Error.Failure("Unable to delete role."));
    }
}