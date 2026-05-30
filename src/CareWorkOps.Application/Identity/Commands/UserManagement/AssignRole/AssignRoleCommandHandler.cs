using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.AssignRole;

public sealed class AssignRoleCommandHandler
    : IRequestHandler<AssignRoleCommand, Result>
{
    private readonly IUserManagementService _userManagementService;

    public AssignRoleCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result> Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _userManagementService.AssignRoleAsync(
            request.TenantId,
            request.UserId,
            request.Role,
            cancellationToken);

        return success
            ? Result.Success()
            : Result.Failure(Error.Failure("Unable to assign role."));
    }
}