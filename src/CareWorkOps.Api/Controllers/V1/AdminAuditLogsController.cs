using Asp.Versioning;
using CareWorkOps.Api.Authorization;
using CareWorkOps.Api.Common;
using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Application.Auditing.Queries.GetAuditLogById;
using CareWorkOps.Application.Auditing.Queries.GetAuditLogs;
using CareWorkOps.Application.Common;
using CareWorkOps.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "TenantAdminOnly")]
[Route("api/v{version:apiVersion}/admin/audit-logs")]
public sealed class AdminAuditLogsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminAuditLogsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [HasPermission(SystemPermissions.AuditView)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<AuditLogDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLogs(
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new GetAuditLogsQuery(tenantId.Value),
            cancellationToken);

        return ToActionResult(
            result,
            "Audit logs retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [HasPermission(SystemPermissions.AuditView)]
    [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuditLogDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAuditLogById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = GetTenantId();

        if (tenantId is null)
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new GetAuditLogByIdQuery(tenantId.Value, id),
            cancellationToken);

        return ToActionResult(
            result,
            "Audit log retrieved successfully.");
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
}