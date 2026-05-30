using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Dtos;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.UpdateTenant
{
    public sealed record UpdateTenantCommand(
    Guid TenantId,
    string Name,
    string IsolationMode,
    string? ConnectionString)
    : IRequest<Result<TenantDto>>;
}
