using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed record LogoutCommand(
    Guid UserId,
    string RefreshToken)
    : IRequest<Result>;