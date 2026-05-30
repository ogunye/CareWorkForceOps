using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication
{
    public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<AuthenticatedUserDto>>;
}
