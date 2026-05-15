using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CareWorkOps.Api.Extensions;

public static class ObservabilityExtensions
{
    private const string ServiceName = "CareWorkOps.Api";
    private const string ServiceVersion = "1.0.0";
    private const string SqlConnectionName = "SqlConnection";

    public static IServiceCollection AddCareWorkOpsObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString(SqlConnectionName);

        if (string.IsNullOrWhiteSpace(sqlConnectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{SqlConnectionName}' was not found.");
        }

        services
            .AddHealthChecks()
            .AddSqlServer(
                connectionString: sqlConnectionString,
                healthQuery: "SELECT 1",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "database", "sql", "sql-server" });

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: ServiceName,
                    serviceVersion: ServiceVersion);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddConsoleExporter();
            });

        return services;
    }
}