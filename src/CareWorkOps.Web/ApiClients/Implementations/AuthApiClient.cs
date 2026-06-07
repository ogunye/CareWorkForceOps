using CareWorkOps.Web.ApiClients.Interfaces;
using CareWorkOps.Web.Infrastructure.Api;
using CareWorkOps.Web.ViewModels.Auth;

namespace CareWorkOps.Web.ApiClients.Implementations;

public sealed class AuthApiClient : BaseApiClient, IAuthApiClient
{
    public AuthApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, httpContextAccessor)
    {
    }

    public Task<ApiResponse<LoginResponseViewModel>> LoginAsync(LoginViewModel model)
    {
        return PostAsync<LoginViewModel, LoginResponseViewModel>(
            "auth/login",
            model);
    }
}