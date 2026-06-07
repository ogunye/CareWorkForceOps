using CareWorkOps.Api.IntegrationTests.Infrastructure;
using CareWorkOps.Application.Identity.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CareWorkOps.Api.IntegrationTests.AdminUsers;

[Collection(nameof(IntegrationTestCollection))]
public sealed class RemoveRoleTests
{
    private readonly HttpClient _client;

    public RemoveRoleTests(CareWorkOpsApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RemoveRole_Should_Return_Ok_When_User_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        var createRequest = new
        {
            firstName = "Role",
            lastName = "Removal",
            email = $"role.removal.{Guid.NewGuid():N}@careworkops.test",
            password = "Password123!",
            roles = new[] { "CareCoordinator", "Supervisor" }
        };

        var createResponse =
            await _client.PostAsJsonAsync("/api/v1/admin/users", createRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser =
            await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();

        var userId = createdUser!.Data!.Id;

        var removeRoleRequest = new
        {
            roleName = "Supervisor"
        };

        var removeResponse =
            await _client.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"/api/v1/admin/users/{userId}/roles")
                {
                    Content = JsonContent.Create(removeRoleRequest)
                });

        removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}