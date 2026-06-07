using System.Security.Claims;
using CareWorkOps.Web.ApiClients.Interfaces;
using CareWorkOps.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CareWorkOps.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthApiClient _authApiClient;

    public AccountController(IAuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _authApiClient.LoginAsync(model);

        if (!response.Success || response.Data is null)
        {
            ModelState.AddModelError(
                string.Empty,
                response.Message ?? "Invalid email or password.");

            return View(model);
        }

        var loginResponse = response.Data;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, loginResponse.FullName),
            new(ClaimTypes.Email, loginResponse.Email),

            // Store tokens as claims
            new("access_token", loginResponse.AccessToken),
            new("refresh_token", loginResponse.RefreshToken)
        };

        foreach (var role in loginResponse.Roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        var identity = new ClaimsIdentity(
            claims,
            "CareWorkOpsCookie");

        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            AllowRefresh = true,
            ExpiresUtc = loginResponse.ExpiresAtUtc
        };

        await HttpContext.SignInAsync(
            "CareWorkOpsCookie",
            principal,
            authProperties);

        // ReturnUrl support
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
            && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        // Default redirect after successful login
        return RedirectToAction(
            actionName: "Index",
            controllerName: "Dashboard");
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CareWorkOpsCookie");

        return RedirectToAction(nameof(Login));
    }
}