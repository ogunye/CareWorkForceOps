using CareWorkOps.Web.ApiClients.Interfaces;
using CareWorkOps.Web.ViewModels.Tenants;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Web.Controllers;

public sealed class TenantsController : Controller
{
    private readonly ITenantApiClient _tenantApiClient;

    public TenantsController(ITenantApiClient tenantApiClient)
    {
        _tenantApiClient = tenantApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] TenantQueryParameters parameters)
    {
        var tenants = await _tenantApiClient.GetTenantsAsync(parameters);

        return View(tenants);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var response = await _tenantApiClient.GetTenantByIdAsync(id);

        if (!response.Success || response.Data is null)
        {
            TempData["ErrorMessage"] = response.Message ?? "Tenant not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(response.Data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateTenantViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTenantViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _tenantApiClient.CreateTenantAsync(model);

        if (!response.Success)
        {
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        TempData["SuccessMessage"] = "Tenant created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        var response = await _tenantApiClient.ActivateTenantAsync(id);

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Tenant activated successfully." : "Unable to activate tenant.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var response = await _tenantApiClient.DeactivateTenantAsync(id);

        TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
            response.Success ? "Tenant deactivated successfully." : "Unable to deactivate tenant.";

        return RedirectToAction(nameof(Index));
    }
}