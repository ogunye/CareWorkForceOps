using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.Authentication;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _handler = new ResetPasswordCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Password_Is_Reset()
    {
        var command = new ResetPasswordCommand(
            "admin@gammacare.com",
            "reset-token",
            "NewPassword123!");

        _authServiceMock
            .Setup(x => x.ResetPasswordAsync(
                command.Email,
                command.ResetToken,
                command.NewPassword,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Reset_Fails()
    {
        var command = new ResetPasswordCommand(
            "admin@alphacare.com",
            "invalid-token",
            "NewPassword123!");

        _authServiceMock
            .Setup(x => x.ResetPasswordAsync(
                command.Email,
                command.ResetToken,
                command.NewPassword,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Unable to reset password.");
    }
}