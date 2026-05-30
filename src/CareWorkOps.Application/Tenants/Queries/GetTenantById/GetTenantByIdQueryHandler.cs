using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Dtos;
using CareWorkOps.Domain.Tenants;
using MediatR;

namespace CareWorkOps.Application.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdQueryHandler
    : IRequestHandler<GetTenantByIdQuery, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Result<TenantDto>> Handle(
        GetTenantByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            TenantId.From(request.TenantId),
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