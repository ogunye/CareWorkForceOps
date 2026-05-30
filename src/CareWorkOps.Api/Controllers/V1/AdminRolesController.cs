using Asp.Versioning;
using CareWorkOps.Api.Common;
using CareWorkOps.Api.Contracts.AdminRoles;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Commands.RoleManagement.AssignPermissionsToRole;
using CareWorkOps.Application.Identity.Commands.RoleManagement.CreateRole;
using CareWorkOps.Application.Identity.Commands.RoleManagement.DeleteRole;
using CareWorkOps.Application.Identity.Commands.RoleManagement.UpdateRole;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Application.Identity.Queries.RoleManagement.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Policy = "TenantAdminOnly")]
    [Route("api/v{version:apiVersion}/admin/roles")]
    public sealed class AdminRolesController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminRolesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<RoleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            var tenantId = GetTenantId();

            if (tenantId is null)
            {
                return Unauthorized();
            }

            var result = await _sender.Send(
                new GetRolesQuery(tenantId.Value),
                cancellationToken);

            return ToActionResult(result, "Roles retrieved successfully.");
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole(
            [FromBody] CreateRoleRequest request,
            CancellationToken cancellationToken)
        {
            var tenantId = GetTenantId();

            if (tenantId is null)
            {
                return Unauthorized();
            }

            var command = new CreateRoleCommand(
                tenantId.Value,
                request.RoleName,
                request.Permissions);

            var result = await _sender.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return ToActionResult(result, "Role created successfully.");
            }

            var response = ApiResponse<RoleDto>.Ok(
                result.Value,
                "Role created successfully.",
                HttpContext.TraceIdentifier);

            return CreatedAtAction(
                nameof(GetRoles),
                new { version = "1.0" },
                response);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRole(
            Guid id,
            [FromBody] UpdateRoleRequest request,
            CancellationToken cancellationToken)
        {
            var tenantId = GetTenantId();

            if (tenantId is null)
            {
                return Unauthorized();
            }

            var command = new UpdateRoleCommand(
                tenantId.Value,
                id,
                request.RoleName);

            var result = await _sender.Send(command, cancellationToken);

            return ToActionResult(result, "Role updated successfully.");
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRole(
            Guid id,
            CancellationToken cancellationToken)
        {
            var tenantId = GetTenantId();

            if (tenantId is null)
            {
                return Unauthorized();
            }

            var result = await _sender.Send(
                new DeleteRoleCommand(tenantId.Value, id),
                cancellationToken);

            return ToActionResult(result, "Role deleted successfully.");
        }

        [HttpPut("{id:guid}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignPermissions(
            Guid id,
            [FromBody] AssignPermissionsRequest request,
            CancellationToken cancellationToken)
        {
            var tenantId = GetTenantId();

            if (tenantId is null)
            {
                return Unauthorized();
            }

            var command = new AssignPermissionsToRoleCommand(
                tenantId.Value,
                id,
                request.Permissions);

            var result = await _sender.Send(command, cancellationToken);

            return ToActionResult(result, "Role permissions updated successfully.");
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
}
