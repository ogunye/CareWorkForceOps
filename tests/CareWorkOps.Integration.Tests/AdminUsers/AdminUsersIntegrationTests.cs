using CareWorkOps.Integration.Tests.Infrastructure;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace CareWorkOps.Integration.Tests.AdminUsers;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AdminUsersIntegrationTests
{
    private readonly HttpClient _client;

    public AdminUsersIntegrationTests(CareWorkOpsApiFactory factory)
    {
        _client = factory.CreateClient();
        TestAuthHelper.ClearAuthorization(_client);
    }

    [Fact]
    public async Task GetUsers_Should_Return_Unauthorized_When_No_Token()
    {
        TestAuthHelper.ClearAuthorization(_client);

        var response = await _client.GetAsync("/api/v1/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Created_When_TenantAdmin_Is_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);

        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(
            _client,
            tenant.AdminEmail,
            tenant.AdminPassword);

        TestAuthHelper.Authorize(_client, token);

        var unique = Guid.NewGuid().ToString("N")[..8];

        var response = await _client.PostAsJsonAsync(
            "/api/v1/admin/users",
            new
            {
                firstName = "Mary",
                lastName = "Jones",
                email = $"mary-{unique}@alphacare.com",
                password = "Password123!",
                roles = new[] { "CareCoordinator" }
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetUsers_Should_Return_Ok_When_TenantAdmin_Is_Authorized()
    {
        var tenant = await TestTenantHelper.CreateTenantAsync(_client);

        var token = await TestAuthHelper.LoginAndGetAccessTokenAsync(
            _client,
            tenant.AdminEmail,
            tenant.AdminPassword);

        TestAuthHelper.Authorize(_client, token);

        var response = await _client.GetAsync("/api/v1/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    //[Fact]
    //public async Task Debug_Login_Token_And_Header()
    //{
    //    var tenant = await TestTenantHelper.CreateTenantAsync(_client);

    //    var loginResponse = await _client.PostAsJsonAsync(
    //        "/api/v1/auth/login",
    //        new
    //        {
    //            email = tenant.AdminEmail,
    //            password = tenant.AdminPassword
    //        });

    //    var loginBody = await loginResponse.Content.ReadAsStringAsync();

    //    loginResponse.StatusCode.Should().Be(HttpStatusCode.OK, loginBody);

    //    using var document = JsonDocument.Parse(loginBody);

    //    var token = document.RootElement
    //        .GetProperty("data")
    //        .GetProperty("accessToken")
    //        .GetString();

    //    token.Should().NotBeNullOrWhiteSpace();

    //    _client.DefaultRequestHeaders.Authorization =
    //        new AuthenticationHeaderValue("Bearer", token);

    //    _client.DefaultRequestHeaders.Authorization.Should().NotBeNull();
    //    _client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
    //    _client.DefaultRequestHeaders.Authorization.Parameter.Should().Be(token);

    //    var handler = new JwtSecurityTokenHandler();
    //    var jwt = handler.ReadJwtToken(token);

    //    jwt.Issuer.Should().Be("CareWorkOps");
    //    jwt.Audiences.Should().Contain("CareWorkOps.Client");

    //    jwt.Claims.Should().Contain(c => c.Type == "TenantId");
    //    jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role || c.Type == "role");

    //    var response = await _client.GetAsync("/api/v1/admin/users");
    //    var body = await response.Content.ReadAsStringAsync();

    //    response.StatusCode.Should().Be(HttpStatusCode.OK, body);

    //}
}