using CareWorkOps.Infrastructure.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CareWorkOps.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCareWorkOpsAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtSettings>(
                configuration.GetSection(JwtSettings.SectionName));

            var jwtSettings = configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>();

            if (jwtSettings is null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JWT settings are missing or invalid.");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1),

                        NameClaimType = "UserId",
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TenantAdminOnly", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("TenantAdmin");
                    policy.RequireClaim("TenantId");
                });
            });

            return services;
        }
    }
}
