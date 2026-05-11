using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation; 

namespace CareWorkOps.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(assembly);
            });

            
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
