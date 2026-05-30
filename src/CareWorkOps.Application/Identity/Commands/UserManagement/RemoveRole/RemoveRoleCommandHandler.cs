using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.RemoveRole;

public sealed class RemoveRoleCommandHandler
    : IRequestHandler<RemoveRoleCommand, Result>
{
    private readonly IUserManagementService _userManagementService;

    public RemoveRoleCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result> Handle(
        RemoveRoleCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _userManagementService.RemoveRoleAsync(
            request.TenantId,
            request.UserId,
            request.Role,
            cancellationToken);

        return success
            ? Result.Success()
            : Result.Failure(Error.Failure("Unable to remove role."));
    }
}