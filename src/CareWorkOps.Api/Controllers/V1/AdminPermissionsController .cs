using Asp.Versioning;
using CareWorkOps.Api.Common;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Application.Identity.Queries.RoleManagement.GetPermissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "TenantAdminOnly")]
[Route("api/v{version:apiVersion}/admin/permissions")]
public sealed class AdminPermissionsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminPermissionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetPermissionsQuery(),
            cancellationToken);

        return ToActionResult(result, "Permissions retrieved successfully.");
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
}