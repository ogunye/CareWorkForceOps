using CareWorkOps.Web.Infrastructure.Api;
using CareWorkOps.Web.ViewModels.Auth;

namespace CareWorkOps.Web.ApiClients.Interfaces;

public interface IAuthApiClient
{
    Task<ApiResponse<LoginResponseViewModel>> LoginAsync(LoginViewModel model);
}