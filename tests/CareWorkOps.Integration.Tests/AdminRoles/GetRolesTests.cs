using System.Net;
using CareWorkOps.Api.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace CareWorkOps.Api.IntegrationTests.AdminRoles;

[Collection(nameof(IntegrationTestCollection))]
public sealed class GetRolesTests
{
    private readonly HttpClient _client;

    public GetRolesTests(CareWorkOpsApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoles_Should_Return_Ok_When_Authorized()
    {
        _client.AuthorizeAsTenantAdmin();

        var response =
            await _client.GetAsync("/api/v1/admin/roles");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRoles_Should_Return_Unauthorized_When_No_Token()
    {
        _client.ClearAuthorization();

        var response =
            await _client.GetAsync("/api/v1/admin/roles");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}