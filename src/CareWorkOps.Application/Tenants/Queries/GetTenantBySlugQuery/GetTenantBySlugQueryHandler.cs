using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Dtos;
using CareWorkOps.Domain.Tenants;
using MediatR;

namespace CareWorkOps.Application.Tenants.Queries.GetTenantBySlug;

public sealed class GetTenantBySlugQueryHandler
    : IRequestHandler<GetTenantBySlugQuery, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantBySlugQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<TenantDto>> Handle(
        GetTenantBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetBySlugAsync(
            request.Slug.Trim().ToLowerInvariant(),
            cancellationToken);

        return tenant is null
            ? Result<TenantDto>.Failure(Error.Failure("Tenant was not found."))
            : Result<TenantDto>.Success(Map(tenant));
    }

    private static TenantDto Map(Tenant tenant)
    {
        return new TenantDto(
            tenant.Id.Value,
            tenant.Name,
            tenant.Slug,
            tenant.IsolationMode,
            tenant.Status,
            tenant.ConnectionString);
    }
}