using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.UserManagement.ReactivateUser;

public sealed class ReactivateUserCommandHandler
    : IRequestHandler<ReactivateUserCommand, Result>
{
    private readonly IUserManagementService _userManagementService;

    public ReactivateUserCommandHandler(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public async Task<Result> Handle(
        ReactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        var success = await _userManagementService.ReactivateUserAsync(
            request.TenantId,
            request.UserId,
            cancellationToken);

        return success
            ? Result.Success()
            : Result.Failure(Error.Failure("Unable to reactivate user."));
    }
}