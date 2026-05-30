namespace CareWorkOps.Api.Contracts.AdminTenants;

public sealed record UpdateTenantRequest(
    string Name,
    string IsolationMode,
    string? ConnectionString);