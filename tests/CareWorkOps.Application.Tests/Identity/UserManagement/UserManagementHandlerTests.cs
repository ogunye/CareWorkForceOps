using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.UserManagement.AssignRole;
using CareWorkOps.Application.Identity.Commands.UserManagement.CreateUser;
using CareWorkOps.Application.Identity.Commands.UserManagement.DeactivateUser;
using CareWorkOps.Application.Identity.Commands.UserManagement.ReactivateUser;
using CareWorkOps.Application.Identity.Commands.UserManagement.RemoveRole;
using CareWorkOps.Application.Identity.Commands.UserManagement.UpdateUser;
using CareWorkOps.Application.Identity.Dtos;
using CareWorkOps.Application.Identity.Queries.UserManagement.GetUserById;
using CareWorkOps.Application.Identity.Queries.UserManagement.GetUsers;
using CareWorkOps.Domain.Identity;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.UserManagement;

public sealed class UserManagementHandlerTests
{
    private readonly Mock<IUserManagementService> _serviceMock = new();

    [Fact]
    public async Task CreateUser_Should_Return_Success()
    {
        var tenantId = Guid.NewGuid();
        var command = new CreateUserCommand(
            tenantId,
            "Mary",
            "Jones",
            "mary@gammacare.com",
            "Password123!",
            ["CareCoordinator"]);

        var user = CreateUserDto(tenantId);

        _serviceMock
            .Setup(x => x.CreateUserAsync(
                tenantId,
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password,
                command.Roles,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new CreateUserCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("mary@gammacare.com");
    }

    [Fact]
    public async Task CreateUser_Should_Return_Failure_When_Service_Returns_Null()
    {
        var command = new CreateUserCommand(
            Guid.NewGuid(),
            "Mary",
            "Jones",
            "mary@gammacare.com",
            "Password123!",
            ["CareCoordinator"]);

        _serviceMock
            .Setup(x => x.CreateUserAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null);

        var handler = new CreateUserCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Unable to create user.");
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Success()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var command = new UpdateUserCommand(
            tenantId,
            userId,
            "Mary",
            "Johnson");

        var user = CreateUserDto(tenantId, userId) with
        {
            LastName = "Johnson"
        };

        _serviceMock
            .Setup(x => x.UpdateUserAsync(
                tenantId,
                userId,
                command.FirstName,
                command.LastName,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new UpdateUserCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.LastName.Should().Be("Johnson");
    }

    [Fact]
    public async Task DeactivateUser_Should_Return_Success()
    {
        var command = new DeactivateUserCommand(Guid.NewGuid(), Guid.NewGuid());

        _serviceMock
            .Setup(x => x.DeactivateUserAsync(
                command.TenantId,
                command.UserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeactivateUserCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ReactivateUser_Should_Return_Success()
    {
        var command = new ReactivateUserCommand(Guid.NewGuid(), Guid.NewGuid());

        _serviceMock
            .Setup(x => x.ReactivateUserAsync(
                command.TenantId,
                command.UserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new ReactivateUserCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task AssignRole_Should_Return_Success()
    {
        var command = new AssignRoleCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CareManager");

        _serviceMock
            .Setup(x => x.AssignRoleAsync(
                command.TenantId,
                command.UserId,
                command.Role,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new AssignRoleCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveRole_Should_Return_Success()
    {
        var command = new RemoveRoleCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CareManager");

        _serviceMock
            .Setup(x => x.RemoveRoleAsync(
                command.TenantId,
                command.UserId,
                command.Role,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new RemoveRoleCommandHandler(_serviceMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetUsers_Should_Return_Users()
    {
        var tenantId = Guid.NewGuid();
        var query = new GetUsersQuery(tenantId);

        _serviceMock
            .Setup(x => x.GetUsersAsync(tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateUserDto(tenantId)]);

        var handler = new GetUsersQueryHandler(_serviceMock.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
    }

    [Fact]
    public async Task GetUserById_Should_Return_User_When_Found()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(tenantId, userId);

        _serviceMock
            .Setup(x => x.GetUserByIdAsync(tenantId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateUserDto(tenantId, userId));

        var handler = new GetUserByIdQueryHandler(_serviceMock.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(userId);
    }

    private static UserDto CreateUserDto(Guid tenantId, Guid? userId = null)
    {
        return new UserDto(
            userId ?? Guid.NewGuid(),
            tenantId,
            "Mary",
            "Jones",
            "mary@gammacare.com",
            UserStatus.Active,
            ["CareCoordinator"]);
    }
}