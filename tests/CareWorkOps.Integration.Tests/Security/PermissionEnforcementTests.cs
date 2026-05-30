using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CareWorkOps.Integration.Tests.Infrastructure;
using CareWorkOps.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace CareWorkOps.Integration.Tests.Security;

[Collection(nameof(IntegrationTestCollection))]
public sealed class PermissionEnforcementTests
{
    private readonly CareWorkOpsApiFactory _factory;
    private readonly HttpClient _client;

    public PermissionEnforcementTests(CareWorkOpsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedEndpoint_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.GetAsync("/api/v1/admin/roles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuditEndpoint_Should_Return_Forbidden_When_User_Lacks_AuditPermission()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var adminToken = await TestAuthHelper.LoginAndGetAccessTokenAsync(
            _client,
            tenant.AdminEmail,
            tenant.AdminPassword);

        TestAuthHelper.Authorize(_client, adminToken);

        var unique = Guid.NewGuid().ToString("N")[..8];

        var createUserResponse = await _client.PostAsJsonAsync("/api/v1/admin/users", new
        {
            firstName = "Limited",
            lastName = "User",
            email = $"limited-{unique}@alphacare.com",
            password = "Password123!",
            roles = new[] { $"LimitedRole-{unique}" }
        });

        createUserResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var limitedToken = await TestAuthHelper.LoginAndGetAccessTokenAsync(
            _client,
            $"limited-{unique}@alphacare.com",
            "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", limitedToken);

        var response = await _client.GetAsync("/api/v1/admin/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}