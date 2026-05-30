using Asp.Versioning;
using CareWorkOps.Api.Common;
using CareWorkOps.Api.Contracts.AdminUsers;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Commands.UserManagement.AssignRole;
using CareWorkOps.Application.Identity.Commands.UserManagement.CreateUser;
using CareWorkOps.Application.Identity.Commands.UserManagement.DeactivateUser;
using CareWorkOps.Application.Identity.Commands.UserManagement.ReactivateUser;
using CareWorkOps.Application.Identity.Commands.UserManagement.RemoveRole;
using CareWorkOps.Application.Identity.Commands.UserManagement.UpdateUser;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Application.Identity.Queries.UserManagement.GetUserById;
using CareWorkOps.Application.Identity.Queries.UserManagement.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "TenantAdminOnly")]
[Route("api/v{version:apiVersion}/admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly ISender _sender;

    public AdminUsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new GetUsersQuery(tenantId.Value),
            cancellationToken);

        return ToActionResult(result, "Users retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new GetUserByIdQuery(tenantId.Value, id),
            cancellationToken);

        return ToActionResult(result, "User retrieved successfully.");
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var command = new CreateUserCommand(
            tenantId.Value,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Roles);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ToActionResult(result, "User created successfully.");
        }

        var response = ApiResponse<UserDto>.Ok(
            result.Value,
            "User created successfully.",
            HttpContext.TraceIdentifier);

        return CreatedAtAction(
            nameof(GetUserById),
            new
            {
                version = "1.0",
                id = result.Value.Id
            },
            response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var command = new UpdateUserCommand(
            tenantId.Value,
            id,
            request.FirstName,
            request.LastName);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(result, "User updated successfully.");
    }

    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeactivateUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new DeactivateUserCommand(tenantId.Value, id),
            cancellationToken);

        return ToActionResult(result, "User deactivated successfully.");
    }

    [HttpPatch("{id:guid}/reactivate")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReactivateUser(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new ReactivateUserCommand(tenantId.Value, id),
            cancellationToken);

        return ToActionResult(result, "User reactivated successfully.");
    }

    [HttpPost("{id:guid}/roles")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignRole(
        Guid id,
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new AssignRoleCommand(tenantId.Value, id, request.Role),
            cancellationToken);

        return ToActionResult(result, "Role assigned successfully.");
    }

    [HttpDelete("{id:guid}/roles/{role}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveRole(
        Guid id,
        string role,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new RemoveRoleCommand(tenantId.Value, id, role),
            cancellationToken);

        return ToActionResult(result, "Role removed successfully.");
    }

    private Guid? GetTenantId()
    {
        var value = User.FindFirst("TenantId")?.Value;

        return Guid.TryParse(value, out var tenantId)
            ? tenantId
            : null;
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
            "Failure.Error" when result.Error.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                => NotFound(response),
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
            "Failure.Error" when result.Error.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                => NotFound(response),
            _ => BadRequest(response)
        };
    }
}