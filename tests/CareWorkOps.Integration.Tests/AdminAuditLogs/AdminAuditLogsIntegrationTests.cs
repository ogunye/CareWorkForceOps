using System.Net;
using CareWorkOps.Integration.Tests.Infrastructure;
using FluentAssertions;

namespace CareWorkOps.Integration.Tests.AdminAuditLogs;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AdminAuditLogsIntegrationTests
{
    private readonly HttpClient _client;

    public AdminAuditLogsIntegrationTests(CareWorkOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuditLogs_Should_Return_Ok_When_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);
        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(_client, tenant.AdminEmail, tenant.AdminPassword);
        TestAuthHelper.Authorize(_client, token);

        var response = await _client.GetAsync("/api/v1/admin/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("LoginSuccess");
    }

    [Fact]
    public async Task GetAuditLogs_Should_Return_Unauthorized_When_No_Token()
    {
        var response = await _client.GetAsync("/api/v1/admin/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}