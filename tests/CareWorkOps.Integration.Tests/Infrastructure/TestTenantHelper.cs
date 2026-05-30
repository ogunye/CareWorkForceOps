using System.Net.Http.Json;
using System.Text.Json;

namespace CareWorkOps.Integration.Tests.Infrastructure;

public static class TestTenantHelper
{
    public static async Task<TestTenantResult> CreateTenantAsync(
        HttpClient client,
        string? slug = null,
        string? adminEmail = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        slug ??= $"alpha-care-{unique}";
        adminEmail ??= $"admin-{unique}@alphacare.com";

        var response = await client.PostAsJsonAsync(
            "/api/v1/tenants",
            new
            {
                tenantName = "Alpha Care Ltd",
                tenantSlug = slug,
                adminFirstName = "Admin",
                adminLastName = "User",
                adminEmail,
                adminPassword = "Password123!",
                isolationMode = "SharedDatabase",
                connectionString = (string?)null
            });

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var data = document.RootElement.GetProperty("data");

        return new TestTenantResult(
            data.GetProperty("tenantId").GetGuid(),
            slug,
            data.GetProperty("adminUserId").GetGuid(),
            adminEmail,
            "Password123!");
    }
}

public sealed record TestTenantResult(
    Guid TenantId,
    string TenantSlug,
    Guid AdminUserId,
    string AdminEmail,
    string AdminPassword);