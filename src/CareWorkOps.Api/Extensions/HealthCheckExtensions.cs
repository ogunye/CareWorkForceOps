using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace CareWorkOps.Api.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IEndpointRouteBuilder MapCareWorkOpsHealthChecks(
            this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration.ToString()
                        }),
                        totalDuration = report.TotalDuration.ToString()
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            response,
                            new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                WriteIndented = true
                            }));
                }
            });

            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("database")
            });

            return endpoints;
        }
    }
}
