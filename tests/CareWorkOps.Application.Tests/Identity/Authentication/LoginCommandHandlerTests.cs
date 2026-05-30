using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.Authentication;
using CareWorkOps.Application.Identity.Dtos;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authenticationServiceMock = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_authenticationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Credentials_Are_Valid()
    {
        var command = new LoginCommand(
            "admin@gammacare.com",
            "Password123!");

        var authenticatedUser = new AuthenticatedUserDto(
            UserId: Guid.NewGuid(),
            TenantId: Guid.NewGuid(),
            Email: "admin@gammacare.com",
            FullName: "Admin User",
            Roles: ["TenantAdmin"],
            AccessToken: "access-token",
            RefreshToken: "refresh-token");

        _authenticationServiceMock
            .Setup(x => x.AuthenticateAsync(
                command.Email,
                command.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(authenticatedUser);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(command.Email);
        result.Value.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Credentials_Are_Invalid()
    {
        var command = new LoginCommand(
            "admin@alphacare.com",
            "WrongPassword");

        _authenticationServiceMock
            .Setup(x => x.AuthenticateAsync(
                command.Email,
                command.Password,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthenticatedUserDto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Invalid email or password.");
    }
}