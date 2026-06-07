using CareWorkOps.Api.IntegrationTests.Infrastructure;
using CareWorkOps.Domain.Identity;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CareWorkOps.Api.IntegrationTests.AdminUsers;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AdminUsersEndpointTests
{
    private readonly HttpClient _client;

    public AdminUsersEndpointTests(CareWorkOpsApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_Should_Return_Ok_When_Authorized()
    {
        _client.AuthorizeAsTenantAdmin();

        var response =
            await _client.GetAsync("/api/v1/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsers_Should_Return_Unauthorized_When_No_Token()
    {
        _client.ClearAuthorization();

        var response =
            await _client.GetAsync("/api/v1/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserById_Should_Return_NotFound_When_User_Does_Not_Exist()
    {
        _client.AuthorizeAsTenantAdmin();

        var userId = Guid.NewGuid();

        var response =
            await _client.GetAsync($"/api/v1/admin/users/{userId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Ok_When_User_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        // Arrange - Create User

        var createRequest = new
        {
            firstName = "Jane",
            lastName = "Original",
            email = $"jane.original.{Guid.NewGuid():N}@careworkops.test",
            password = "Password123!",
            roles = new[] { "CareCoordinator" }
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/v1/admin/users", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser =
            await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        createdUser.Should().NotBeNull();
        createdUser!.Data.Should().NotBeNull();

        var userId = createdUser.Data!.Id;

        // Act - Update User

        var updateRequest = new
        {
            firstName = "Jane",
            lastName = "Updated",
            email = createRequest.email
        };

        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/v1/admin/users/{userId}",
                updateRequest);

        // Assert

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify update

        var getResponse =
            await _client.GetAsync(
                $"/api/v1/admin/users/{userId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedUser =
            await getResponse.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        updatedUser.Should().NotBeNull();
        updatedUser!.Data.Should().NotBeNull();

        updatedUser.Data!.LastName.Should().Be("Updated");
    }


    [Fact]
    public async Task DeactivateUser_Should_Return_Ok_When_User_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        // Arrange - Create User
        var createRequest = new
        {
            firstName = "Mark",
            lastName = "Active",
            email = $"mark.active.{Guid.NewGuid():N}@careworkops.test",
            password = "Password123!",
            roles = new[] { "CareCoordinator" }
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/v1/admin/users", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser =
            await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        createdUser.Should().NotBeNull();
        createdUser!.Data.Should().NotBeNull();

        var userId = createdUser.Data!.Id;

        // Act - Deactivate User
        var deactivateResponse =
            await _client.PatchAsync(
                $"/api/v1/admin/users/{userId}/deactivate",
                content: null);

        // Assert
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }


    [Fact]
    public async Task ReactivateUser_Should_Return_Ok_When_User_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        var createRequest = new
        {
            firstName = "Sarah",
            lastName = "Inactive",
            email = $"sarah.inactive.{Guid.NewGuid():N}@careworkops.test",
            password = "Password123!",
            roles = new[] { "CareCoordinator" }
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/v1/admin/users", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser =
            await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        var userId = createdUser!.Data!.Id;

        var deactivateResponse =
            await _client.PatchAsync($"/api/v1/admin/users/{userId}/deactivate", null);

        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reactivateResponse =
            await _client.PatchAsync($"/api/v1/admin/users/{userId}/reactivate", null);

        reactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserById_Should_Return_Ok_When_User_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        var createRequest = new
        {
            firstName = "John",
            lastName = "Tester",
            email = $"john.tester.{Guid.NewGuid():N}@careworkops.test",
            password = "Password123!",
            roles = new[] { "CareCoordinator" }
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/v1/admin/users", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser =
            await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        createdUser.Should().NotBeNull();
        createdUser!.Data.Should().NotBeNull();

        var userId = createdUser.Data!.Id;

        var getResponse =
            await _client.GetAsync($"/api/v1/admin/users/{userId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Created_When_Request_Is_Valid()
    {
        _client.AuthorizeAsTenantAdmin();

        var request = new
        {
            firstName = "John",
            lastName = "Smith",
            email = $"john.smith.{Guid.NewGuid():N}@careworkops.test",
            password = "Password123!",
            roles = new[] { "CareCoordinator" }
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/admin/users",
                request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();

        apiResponse.Data!.FirstName.Should().Be(request.firstName);
        apiResponse.Data.LastName.Should().Be(request.lastName);
        apiResponse.Data.Email.Should().Be(request.email);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Conflict_When_Email_Already_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        var email = $"duplicate.user.{Guid.NewGuid():N}@careworkops.test";

        var request = new
        {
            firstName = "Duplicate",
            lastName = "User",
            email,
            password = "Password123!",
            roles = new[] { "CareCoordinator" }
        };

        var firstResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/admin/users",
                request);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/admin/users",
                request);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var secondBody = await secondResponse.Content.ReadAsStringAsync();

        secondResponse.StatusCode.Should().Be(
            HttpStatusCode.Conflict,
            $"response body was: {secondBody}");
    }

    [Fact]
    public async Task CreateUser_Should_Return_BadRequest_When_Request_Is_Invalid()
    {
        _client.AuthorizeAsTenantAdmin();

        var request = new
        {
            firstName = "",
            lastName = "",
            email = "not-an-email",
            password = "123",
            roles = Array.Empty<string>()
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/v1/admin/users",
                request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }





    private sealed class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    private sealed class UserDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}