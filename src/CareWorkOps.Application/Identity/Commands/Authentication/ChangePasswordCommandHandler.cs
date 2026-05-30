using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IAuthenticationService _authenticationService;

    public ChangePasswordCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var changed = await _authenticationService.ChangePasswordAsync(
            request.UserId,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        return changed
            ? Result.Success()
            : Result.Failure(Error.Validation("Unable to change password."));
    }
}