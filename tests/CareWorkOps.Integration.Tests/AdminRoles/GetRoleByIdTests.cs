using CareWorkOps.Api.IntegrationTests.Infrastructure;
using CareWorkOps.Application.Identity.Dtos;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using RoleDto = CareWorkOps.Api.IntegrationTests.Infrastructure.RoleDto;

namespace CareWorkOps.Api.IntegrationTests.AdminRoles;

[Collection(nameof(IntegrationTestCollection))]
public sealed class GetRoleByIdTests
{
    private readonly HttpClient _client;

    public GetRoleByIdTests(CareWorkOpsApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoleById_Should_Return_NotFound_When_Role_Does_Not_Exist()
    {
        _client.AuthorizeAsTenantAdmin();

        var roleId = Guid.NewGuid();

        var response =
            await _client.GetAsync(
                $"/api/v1/admin/roles/{roleId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRoleById_Should_Return_Ok_When_Role_Exists()
    {
        _client.AuthorizeAsTenantAdmin();

        // Create Role First
        var createRoleRequest = new
        {
            name = $"Supervisor-{Guid.NewGuid():N}"
        };

        var createResponse =
            await _client.PostAsJsonAsync(
                "/api/v1/admin/roles",
                createRoleRequest);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdRole =
            await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        createdRole.Should().NotBeNull();
        createdRole!.Data.Should().NotBeNull();

        var roleId = createdRole.Data!.Id;

        // Retrieve Role
        var getResponse =
            await _client.GetAsync(
                $"/api/v1/admin/roles/{roleId}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var role =
            await getResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        role.Should().NotBeNull();
        role!.Data.Should().NotBeNull();
        role.Data.Id.Should().Be(roleId);
    }
}