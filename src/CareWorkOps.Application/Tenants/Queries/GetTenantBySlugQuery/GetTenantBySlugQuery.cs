using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Dtos;
using MediatR;

namespace CareWorkOps.Application.Tenants.Queries.GetTenantBySlug;

public sealed record GetTenantBySlugQuery(string Slug) : IRequest<Result<TenantDto>>;