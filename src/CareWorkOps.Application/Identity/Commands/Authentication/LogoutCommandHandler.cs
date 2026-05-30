using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthenticationService _authenticationService;

    public LogoutCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var revoked = await _authenticationService.RevokeRefreshTokenAsync(
            request.UserId,
            request.RefreshToken,
            cancellationToken);

        return revoked
            ? Result.Success()
            : Result.Failure(Error.Validation("Invalid refresh token."));
    }
}