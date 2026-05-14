namespace CareWorkOps.Api.Contracts.Tenants
{
    public sealed record CreateTenantRequest(
    string TenantName,
    string TenantSlug,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword,
    string IsolationMode,
    string? ConnectionString);
}
