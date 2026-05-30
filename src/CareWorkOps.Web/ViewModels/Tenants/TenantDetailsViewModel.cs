namespace CareWorkOps.Web.ViewModels.Tenants;

public sealed class TenantDetailsViewModel
{
    public Guid Id { get; set; }

    public string TenantName { get; set; } = string.Empty;

    public string TenantSlug { get; set; } = string.Empty;

    public string IsolationMode { get; set; } = string.Empty;

    public string? ConnectionString { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}