using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.DeactivateUser;

public sealed class DeactivateUserCommandHandler
    : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IUserManagementService _userManagementService;

    public DeactivateUserCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result> Handle(
        DeactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _userManagementService.DeactivateUserAsync(
            request.TenantId,
            request.UserId,
            cancellationToken);

        return success
            ? Result.Success()
            : Result.Failure(Error.Failure("Unable to deactivate user."));
    }
}