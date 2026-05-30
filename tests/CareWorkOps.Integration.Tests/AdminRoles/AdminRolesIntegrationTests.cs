using System.Net;
using System.Net.Http.Json;
using CareWorkOps.Integration.Tests.Infrastructure;
using FluentAssertions;

namespace CareWorkOps.Integration.Tests.AdminRoles;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AdminRolesIntegrationTests
{
    private readonly HttpClient _client;

    public AdminRolesIntegrationTests(CareWorkOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateRole_Should_Return_Created_When_TenantAdmin_Has_Permission()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(_client, tenant.AdminEmail, tenant.AdminPassword);
        TestAuthHelper.Authorize(_client, token);

        var response = await _client.PostAsJsonAsync("/api/v1/admin/roles", new
        {
            roleName = "CareCoordinator",
            permissions = new[] { "users.view", "users.create" }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetPermissions_Should_Return_Ok_When_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(_client, tenant.AdminEmail, tenant.AdminPassword);
        TestAuthHelper.Authorize(_client, token);

        var response = await _client.GetAsync("/api/v1/admin/permissions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("users.view");
        json.Should().Contain("audit.view");
    }
}