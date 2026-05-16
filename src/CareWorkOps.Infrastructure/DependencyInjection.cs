using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Infrastructure.Identity;
using CareWorkOps.Infrastructure.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareWorkOps.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
