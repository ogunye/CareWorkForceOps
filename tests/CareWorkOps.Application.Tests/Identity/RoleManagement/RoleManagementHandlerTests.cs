using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.RoleManagement.AssignPermissionsToRole;
using CareWorkOps.Application.Identity.Commands.RoleManagement.CreateRole;
using CareWorkOps.Application.Identity.Commands.RoleManagement.DeleteRole;
using CareWorkOps.Application.Identity.Commands.RoleManagement.UpdateRole;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Application.Identity.Queries.RoleManagement.GetPermissions;
using CareWorkOps.Application.Identity.Queries.RoleManagement.GetRoles;
using CareWorkOps.Domain.Identity;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.RoleManagement;

public sealed class RoleManagementHandlerTests
{
    private readonly Mock<IRoleManagementService> _serviceMock = new();

    [Fact]
    public async Task CreateRole_Should_Return_Success()
    {
        var tenantId = Guid.NewGuid();

        var command = new CreateRoleCommand(
            tenantId,
            "CareCoordinator",
            [SystemPermissions.UsersView]);

        var role = new RoleDto(
            Guid.NewGuid(),
            tenantId,
            "CareCoordinator",
            [SystemPermissions.UsersView]);

        _serviceMock
            .Setup(x => x.CreateRoleAsync(
                tenantId,
                command.RoleName,
                command.Permissions,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = new CreateRoleCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("CareCoordinator");
    }

    [Fact]
    public async Task UpdateRole_Should_Return_Success()
    {
        var tenantId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var command = new UpdateRoleCommand(
            tenantId,
            roleId,
            "SeniorCareCoordinator");

        var role = new RoleDto(
            roleId,
            tenantId,
            "SeniorCareCoordinator",
            []);

        _serviceMock
            .Setup(x => x.UpdateRoleAsync(
                tenantId,
                roleId,
                command.RoleName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = new UpdateRoleCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("SeniorCareCoordinator");
    }

    [Fact]
    public async Task DeleteRole_Should_Return_Success()
    {
        var command = new DeleteRoleCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        _serviceMock
            .Setup(x => x.DeleteRoleAsync(
                command.TenantId,
                command.RoleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteRoleCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AssignPermissions_Should_Return_Success()
    {
        var tenantId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var command = new AssignPermissionsToRoleCommand(
            tenantId,
            roleId,
            [SystemPermissions.UsersView, SystemPermissions.UsersCreate]);

        var role = new RoleDto(
            roleId,
            tenantId,
            "CareCoordinator",
            command.Permissions);

        _serviceMock
            .Setup(x => x.AssignPermissionsAsync(
                tenantId,
                roleId,
                command.Permissions,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = new AssignPermissionsToRoleCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Permissions.Should().Contain(SystemPermissions.UsersCreate);
    }

    [Fact]
    public async Task GetRoles_Should_Return_Roles()
    {
        var tenantId = Guid.NewGuid();
        var query = new GetRolesQuery(tenantId);

        _serviceMock
            .Setup(x => x.GetRolesAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new RoleDto(Guid.NewGuid(), tenantId, "CareCoordinator", [])
            ]);

        var handler = new GetRolesQueryHandler(_serviceMock.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
    }

    [Fact]
    public async Task GetPermissions_Should_Return_Permissions()
    {
        var query = new GetPermissionsQuery();

        _serviceMock
            .Setup(x => x.GetPermissionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new PermissionDto(
                    SystemPermissions.UsersView,
                    "View Users",
                    PermissionGroup.UserManagement,
                    "View users.")
            ]);

        var handler = new GetPermissionsQueryHandler(_serviceMock.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
    }
}