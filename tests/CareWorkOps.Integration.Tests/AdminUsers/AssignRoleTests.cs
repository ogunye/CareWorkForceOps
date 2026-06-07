using CareWorkOps.Api.IntegrationTests.Infrastructure;
using CareWorkOps.Application.Identity.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CareWorkOps.Api.IntegrationTests.AdminUsers;

[Collection(nameof(IntegrationTestCollection))]
public sealed class AssignRoleTests
{
    private readonly HttpClient _client;

    public AssignRoleTests(CareWorkOpsApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AssignRole_Should_Return_Ok_When_User_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        var createRequest = new
        {
            firstName = "Role",
            lastName = "Assignee",
            email = $"role.assignee.{Guid.NewGuid():N}@careworkops.test",
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

        var assignRoleRequest = new
        {
            roleName = "Supervisor"
        };

        var assignResponse =
            await _client.PostAsJsonAsync(
                $"/api/v1/admin/users/{userId}/roles",
                assignRoleRequest);

        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}