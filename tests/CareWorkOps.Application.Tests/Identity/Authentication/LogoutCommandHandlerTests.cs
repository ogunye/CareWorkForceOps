using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.Authentication;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class LogoutCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _handler = new LogoutCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_RefreshToken_Is_Revoked()
    {
        var command = new LogoutCommand(Guid.NewGuid(), "refresh-token");

        _authServiceMock
            .Setup(x => x.RevokeRefreshTokenAsync(
                command.UserId,
                command.RefreshToken,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_RefreshToken_Is_Invalid()
    {
        var command = new LogoutCommand(Guid.NewGuid(), "invalid-token");

        _authServiceMock
            .Setup(x => x.RevokeRefreshTokenAsync(
                command.UserId,
                command.RefreshToken,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Invalid refresh token.");
    }
}