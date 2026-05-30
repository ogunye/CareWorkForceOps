using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IAuthenticationService _authenticationService;

    public ForgotPasswordCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        await _authenticationService.GeneratePasswordResetTokenAsync(
            request.Email,
            cancellationToken);

        return Result.Success();
    }
}