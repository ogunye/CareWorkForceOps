using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed record ForgotPasswordCommand(
    string Email)
    : IRequest<Result>;