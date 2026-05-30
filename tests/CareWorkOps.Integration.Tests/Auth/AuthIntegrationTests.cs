using System.Net;
using System.Net.Http.Json;
using CareWorkOps.Integration.Tests.Infrastructure;
using FluentAssertions;

namespace CareWorkOps.Integration.Tests.Auth;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AuthIntegrationTests
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CareWorkOpsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Should_Return_AccessToken_And_RefreshToken()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email = tenant.AdminEmail,
                password = tenant.AdminPassword
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();

        json.Should().Contain("accessToken");
        json.Should().Contain("refreshToken");
        json.Should().Contain("Login successful");
    }

    [Fact]
    public async Task Login_Should_Return_BadRequest_When_Credentials_Are_Invalid()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email = tenant.AdminEmail,
                password = "WrongPassword123!"
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}