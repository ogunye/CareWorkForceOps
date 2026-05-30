using Asp.Versioning;
using CareWorkOps.Api.Common;
using CareWorkOps.Api.Contracts.Auth;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Commands.Authentication;
using CareWorkOps.Application.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareWorkOps.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticatedUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthenticatedUserDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(
            result,
            "Login successful.");
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticatedUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthenticatedUserDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(
            result,
            "Token refreshed successfully.");
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var command = new LogoutCommand(
            userId.Value,
            request.RefreshToken);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(
            result,
            "Logout successful.");
    }

    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var command = new ChangePasswordCommand(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(
            result,
            "Password changed successfully.");
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ForgotPasswordCommand(request.Email);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(
            result,
            "If the email exists, password reset instructions will be sent.");
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(
            request.Email,
            request.ResetToken,
            request.NewPassword);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(
            result,
            "Password reset successfully.");
    }

    private IActionResult ToActionResult<T>(
        Result<T> result,
        string successMessage)
    {
        var correlationId = HttpContext.TraceIdentifier;

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(
                result.Value,
                successMessage,
                correlationId));
        }

        var response = ApiResponse<T>.Fail(
            result.Error.Message,
            [result.Error.Message],
            correlationId);

        return result.Error.Code switch
        {
            "Validation.Error" => BadRequest(response),
            "Conflict.Error" => Conflict(response),
            _ => BadRequest(response)
        };
    }

    private IActionResult ToActionResult(
        Result result,
        string successMessage)
    {
        var correlationId = HttpContext.TraceIdentifier;

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<object>.Ok(
                new { },
                successMessage,
                correlationId));
        }

        var response = ApiResponse<object>.Fail(
            result.Error.Message,
            [result.Error.Message],
            correlationId);

        return result.Error.Code switch
        {
            "Validation.Error" => BadRequest(response),
            "Conflict.Error" => Conflict(response),
            _ => BadRequest(response)
        };
    }

    private Guid? GetCurrentUserId()
    {
        var value =
            User.FindFirstValue("UserId") ??
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        return Guid.TryParse(value, out var userId)
            ? userId
            : null;
    }
}