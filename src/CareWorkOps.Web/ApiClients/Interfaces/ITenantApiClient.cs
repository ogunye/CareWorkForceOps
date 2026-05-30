using CareWorkOps.Web.Infrastructure.Api;
using CareWorkOps.Web.ViewModels.Tenants;

namespace CareWorkOps.Web.ApiClients.Interfaces;

public interface ITenantApiClient
{
    Task<PagedResult<TenantListItemViewModel>> GetTenantsAsync(
        TenantQueryParameters parameters);

    Task<ApiResponse<TenantDetailsViewModel>> GetTenantByIdAsync(Guid id);

    Task<ApiResponse<TenantDetailsViewModel>> CreateTenantAsync(
        CreateTenantViewModel model);

    Task<ApiResponse<bool>> ActivateTenantAsync(Guid id);

    Task<ApiResponse<bool>> DeactivateTenantAsync(Guid id);
}