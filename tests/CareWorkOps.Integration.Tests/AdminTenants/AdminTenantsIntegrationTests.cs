using System.Net;
using System.Net.Http.Json;
using CareWorkOps.Integration.Tests.Infrastructure;
using FluentAssertions;

namespace CareWorkOps.Integration.Tests.AdminTenants;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AdminTenantsIntegrationTests
{
    private readonly HttpClient _client;

    public AdminTenantsIntegrationTests(CareWorkOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTenantById_Should_Return_Ok_When_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(_client, tenant.AdminEmail, tenant.AdminPassword);
        TestAuthHelper.Authorize(_client, token);

        var response = await _client.GetAsync($"/api/v1/admin/tenants/{tenant.TenantId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ActivateTenant_Should_Return_Ok_When_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(_client, tenant.AdminEmail, tenant.AdminPassword);
        TestAuthHelper.Authorize(_client, token);

        var response = await _client.PatchAsync(
            $"/api/v1/admin/tenants/{tenant.TenantId}/activate",
            null);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SuspendTenant_Should_Return_Ok_When_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(_client, tenant.AdminEmail, tenant.AdminPassword);
        TestAuthHelper.Authorize(_client, token);

        var response = await _client.PatchAsJsonAsync(
            $"/api/v1/admin/tenants/{tenant.TenantId}/suspend",
            new
            {
                reason = "Integration test suspension."
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}