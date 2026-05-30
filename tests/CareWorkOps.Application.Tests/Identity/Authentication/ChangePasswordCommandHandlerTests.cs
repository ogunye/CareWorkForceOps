using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.Authentication;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Password_Is_Changed()
    {
        var command = new ChangePasswordCommand(
            Guid.NewGuid(),
            "OldPassword123!",
            "NewPassword123!");

        _authServiceMock
            .Setup(x => x.ChangePasswordAsync(
                command.UserId,
                command.CurrentPassword,
                command.NewPassword,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Password_Change_Fails()
    {
        var command = new ChangePasswordCommand(
            Guid.NewGuid(),
            "WrongPassword123!",
            "NewPassword123!");

        _authServiceMock
            .Setup(x => x.ChangePasswordAsync(
                command.UserId,
                command.CurrentPassword,
                command.NewPassword,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Unable to change password.");
    }
}