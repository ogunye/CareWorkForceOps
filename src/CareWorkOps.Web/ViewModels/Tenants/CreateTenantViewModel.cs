using System.ComponentModel.DataAnnotations;

namespace CareWorkOps.Web.ViewModels.Tenants;

public sealed class CreateTenantViewModel
{
    [Required]
    [Display(Name = "Tenant Name")]
    public string TenantName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Tenant Slug")]
    public string TenantSlug { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Admin First Name")]
    public string AdminFirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Admin Last Name")]
    public string AdminLastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Admin Email")]
    public string AdminEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Admin Password")]
    public string AdminPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Isolation Mode")]
    public string IsolationMode { get; set; } = "SharedDatabase";

    public string? ConnectionString { get; set; }
}