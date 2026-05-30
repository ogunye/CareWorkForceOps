namespace CareWorkOps.Web.ViewModels.Tenants;

public sealed class TenantQueryParameters
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SearchTerm { get; set; }

    public bool? IsActive { get; set; }
}