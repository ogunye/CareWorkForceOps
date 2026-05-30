using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Identity.Commands.Authentication;

public sealed record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword)
    : IRequest<Result>;