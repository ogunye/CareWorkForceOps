using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthenticatedUserDto>>
{
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<AuthenticatedUserDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _authenticationService.RefreshTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (user is null)
        {
            return Result<AuthenticatedUserDto>.Failure(
                Error.Validation("Invalid or expired refresh token."));
        }

        return Result<AuthenticatedUserDto>.Success(user);
    }
}