using CareWorkOps.Web.ApiClients.Interfaces;
using CareWorkOps.Web.Infrastructure.Api;
using CareWorkOps.Web.ViewModels.Tenants;

namespace CareWorkOps.Web.ApiClients.Implementations;

public sealed class TenantApiClient : BaseApiClient, ITenantApiClient
{
    public TenantApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, httpContextAccessor)
    {
    }

    public Task<PagedResult<TenantListItemViewModel>> GetTenantsAsync(
        TenantQueryParameters parameters)
    {
        var query = BuildTenantQuery(parameters);

        return GetPagedAsync<TenantListItemViewModel>($"tenants{query}");
    }

    public Task<ApiResponse<TenantDetailsViewModel>> GetTenantByIdAsync(Guid id)
    {
        return GetAsync<TenantDetailsViewModel>($"tenants/{id}");
    }

    public Task<ApiResponse<TenantDetailsViewModel>> CreateTenantAsync(
        CreateTenantViewModel model)
    {
        return PostAsync<CreateTenantViewModel, TenantDetailsViewModel>(
            "tenants",
            model);
    }

    public Task<ApiResponse<bool>> ActivateTenantAsync(Guid id)
    {
        return PutAsync<object, bool>(
            $"tenants/{id}/activate",
            new { });
    }

    public Task<ApiResponse<bool>> DeactivateTenantAsync(Guid id)
    {
        return PutAsync<object, bool>(
            $"tenants/{id}/deactivate",
            new { });
    }

    private static string BuildTenantQuery(TenantQueryParameters parameters)
    {
        var query = new List<string>
        {
            $"pageNumber={parameters.PageNumber}",
            $"pageSize={parameters.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            query.Add($"searchTerm={Uri.EscapeDataString(parameters.SearchTerm)}");
        }

        if (parameters.IsActive.HasValue)
        {
            query.Add($"isActive={parameters.IsActive.Value}");
        }

        return $"?{string.Join("&", query)}";
    }
}