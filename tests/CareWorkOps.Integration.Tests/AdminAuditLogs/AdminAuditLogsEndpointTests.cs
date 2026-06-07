using System.Net;
using CareWorkOps.Api.IntegrationTests.Infrastructure;
using CareWorkOps.Domain.Identity;
using FluentAssertions;

namespace CareWorkOps.Api.IntegrationTests.AdminAuditLogs;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AdminAuditLogsEndpointTests
{
    private readonly CareWorkOpsApiTestFactory _factory;
    private readonly HttpClient _client;

    public AdminAuditLogsEndpointTests(CareWorkOpsApiTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuditLogs_Should_Return_Ok_When_Authorized()
    {
        _client.AuthorizeAsTenantAdmin(SystemPermissions.AuditView);

        var response = await _client.GetAsync("/api/v1/admin/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAuditLogs_Should_Return_Unauthorized_When_No_Token()
    {
        _client.ClearAuthorization();

        var response = await _client.GetAsync("/api/v1/admin/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAuditLogs_Should_Return_Forbidden_When_Missing_AuditView_Permission()
    {
        _client.AuthorizeAsTenantAdmin("Some.Other.Permission");

        var response = await _client.GetAsync("/api/v1/admin/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAuditLogById_Should_Return_NotFound_When_Log_Does_Not_Exist()
    {
        _client.AuthorizeAsTenantAdmin(SystemPermissions.AuditView);

        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/v1/admin/audit-logs/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


}