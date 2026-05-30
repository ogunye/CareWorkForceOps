using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication
{
    public sealed class LoginCommandHandler
     : IRequestHandler<LoginCommand, Result<AuthenticatedUserDto>>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthenticatedUserDto>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _authenticationService.AuthenticateAsync(
                request.Email,
                request.Password,
                cancellationToken);

            if (user is null)
            {
                return Result<AuthenticatedUserDto>.Failure(
                    Error.Validation("Invalid email or password."));
            }

            return Result<AuthenticatedUserDto>.Success(user);
        }
    }
}
