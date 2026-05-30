using Asp.Versioning;
using CareWorkOps.Api.Authorization;
using CareWorkOps.Api.Common;
using CareWorkOps.Api.Contracts.AdminTenants;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Commands.ActivateTenant;
using CareWorkOps.Application.Tenants.Commands.ArchiveTenant;
using CareWorkOps.Application.Tenants.Commands.SuspendTenant;
using CareWorkOps.Application.Tenants.Commands.UpdateTenant;
using CareWorkOps.Application.Tenants.Dtos;
using CareWorkOps.Application.Tenants.Queries.GetTenantById;
using CareWorkOps.Application.Tenants.Queries.GetTenantBySlug;
using CareWorkOps.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "TenantAdminOnly")]
[Route("api/v{version:apiVersion}/admin/tenants")]
public sealed class AdminTenantsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminTenantsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    [HasPermission(SystemPermissions.TenantsView)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetTenantByIdQuery(id),
            cancellationToken);

        return ToActionResult(result, "Tenant retrieved successfully.");
    }

    [HttpGet("by-slug/{slug}")]
    [HasPermission(SystemPermissions.TenantsView)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetTenantBySlugQuery(slug),
            cancellationToken);

        return ToActionResult(result, "Tenant retrieved successfully.");
    }

    [HttpPut("{id:guid}")]
    [HasPermission(SystemPermissions.TenantsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTenant(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTenantCommand(
            TenantId: id,
            Name: request.Name,
            IsolationMode: request.IsolationMode,
            ConnectionString: request.ConnectionString);

        var result = await _sender.Send(command, cancellationToken);

        return ToActionResult(result, "Tenant updated successfully.");
    }

    [HttpPatch("{id:guid}/activate")]
    [HasPermission(SystemPermissions.TenantsActivate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateTenant(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ActivateTenantCommand(id),
            cancellationToken);

        return ToActionResult(result, "Tenant activated successfully.");
    }

    [HttpPatch("{id:guid}/suspend")]
    [HasPermission(SystemPermissions.TenantsSuspend)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SuspendTenant(
        Guid id,
        [FromBody] SuspendTenantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new SuspendTenantCommand(id, request.Reason),
            cancellationToken);

        return ToActionResult(result, "Tenant suspended successfully.");
    }

    [HttpPatch("{id:guid}/archive")]
    [HasPermission(SystemPermissions.TenantsSuspend)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveTenant(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ArchiveTenantCommand(id),
            cancellationToken);

        return ToActionResult(result, "Tenant archived successfully.");
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