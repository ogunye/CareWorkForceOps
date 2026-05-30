using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Dtos;
using MediatR;

namespace CareWorkOps.Application.Tenants.Queries.GetTenantById;

public sealed record GetTenantByIdQuery(Guid TenantId) : IRequest<Result<TenantDto>>;