namespace CareWorkOps.Web.ViewModels.Tenants;

public sealed class TenantListItemViewModel
{
    public Guid Id { get; set; }

    public string TenantName { get; set; } = string.Empty;

    public string TenantSlug { get; set; } = string.Empty;

    public string IsolationMode { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}