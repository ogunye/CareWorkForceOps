using CareWorkOps.Api.Authorization;
using CareWorkOps.Domain.Identity;
using CareWorkOps.Infrastructure.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CareWorkOps.Api.Extensions;

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

        if (jwtSettings is null ||
            string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
        {
            throw new InvalidOperationException("JWT settings are missing or invalid.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 bytes / 256 bits.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidIssuer = jwtSettings.Issuer,

                   ValidateAudience = true,
                   ValidAudience = jwtSettings.Audience,

                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.FromMinutes(1),

                   NameClaimType = "UserId",
                   RoleClaimType = ClaimTypes.Role
               };

               options.Events = new JwtBearerEvents
               {
                   OnMessageReceived = context =>
                   {
                       Console.WriteLine("JWT MESSAGE RECEIVED:");
                       Console.WriteLine(context.Token ?? "No token found");
                       return Task.CompletedTask;
                   },

                   OnAuthenticationFailed = context =>
                   {
                       Console.WriteLine("JWT AUTHENTICATION FAILED:");
                       Console.WriteLine(context.Exception.Message);
                       return Task.CompletedTask;
                   },

                   OnTokenValidated = context =>
                   {
                       Console.WriteLine("JWT TOKEN VALIDATED SUCCESSFULLY");

                       foreach (var claim in context.Principal!.Claims)
                       {
                           Console.WriteLine($"{claim.Type}: {claim.Value}");
                       }

                       return Task.CompletedTask;
                   },

                   OnChallenge = context =>
                   {
                       Console.WriteLine("JWT CHALLENGE:");
                       Console.WriteLine($"Error: {context.Error}");
                       Console.WriteLine($"Description: {context.ErrorDescription}");
                       return Task.CompletedTask;
                   }
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

            foreach (var permission in PermissionCatalogue.All)
            {
                options.AddPolicy(permission.Code, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("TenantId");
                    policy.AddRequirements(
                        new PermissionRequirement(permission.Code));
                });
            }
        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        return services;
    }
}