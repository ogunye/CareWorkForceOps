using Asp.Versioning;
using CareWorkOps.Api.Common;
using CareWorkOps.Api.Contracts.Tenants;
using CareWorkOps.Application.Tenants.Commands.CreateTenant;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tenants")]
    [ApiController]
    public sealed class TenantsController : ControllerBase
    {
        private readonly ISender _sender;

        public TenantsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreateTenantResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<CreateTenantResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<CreateTenantResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateTenant(
            [FromBody] CreateTenantRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateTenantCommand(
                TenantName: request.TenantName,
                TenantSlug: request.TenantSlug,
                AdminFirstName: request.AdminFirstName,
                AdminLastName: request.AdminLastName,
                AdminEmail: request.AdminEmail,
                AdminPassword: request.AdminPassword,
                IsolationMode: request.IsolationMode,
                ConnectionString: request.ConnectionString);

            var result = await _sender.Send(command, cancellationToken);

            var correlationId = HttpContext.TraceIdentifier;

            if (result.IsSuccess)
            {
                var response = ApiResponse<CreateTenantResponse>.Ok(
                    result.Value,
                    "Tenant created successfully.",
                    correlationId);

                return CreatedAtAction(
                    nameof(CreateTenant),
                    new
                    {
                        version = "1.0",
                        tenantId = result.Value.TenantId
                    },
                    response);
            }

            var errorResponse = ApiResponse<CreateTenantResponse>.Fail(
                result.Error.Message,
                [result.Error.Message],
                correlationId);

            return result.Error.Code switch
            {
                "Conflict.Error" => Conflict(errorResponse),
                "Validation.Error" => BadRequest(errorResponse),
                _ => BadRequest(errorResponse)
            };
        }
    }
}
