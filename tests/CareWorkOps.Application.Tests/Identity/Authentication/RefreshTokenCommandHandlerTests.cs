using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.Authentication;
using CareWorkOps.Application.Identity.Dtos;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_RefreshToken_Is_Valid()
    {
        var command = new RefreshTokenCommand("valid-refresh-token");

        var user = new AuthenticatedUserDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "admin@gammacare.com",
            "Admin User",
            ["TenantAdmin"],
            "new-access-token",
            "new-refresh-token");

        _authServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("new-access-token");
        result.Value.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_RefreshToken_Is_Invalid()
    {
        var command = new RefreshTokenCommand("invalid-refresh-token");

        _authServiceMock
            .Setup(x => x.RefreshTokenAsync(command.RefreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthenticatedUserDto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Invalid or expired refresh token.");
    }
}