using CareWorkOps.Domain.Tenants;

namespace CareWorkOps.Application.Abstractions.Persistence
{
    public interface ITenantRepository
    {
        Task<bool> ExistsBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Tenant tenant,
            CancellationToken cancellationToken = default);

        Task<Tenant?> GetByIdAsync(
            TenantId tenantId,
            CancellationToken cancellationToken = default);

        Task<Tenant?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);
    }
}
