using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Identity.Commands.Authentication;
using FluentAssertions;
using Moq;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IAuthenticationService> _authServiceMock = new();
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _handler = new ForgotPasswordCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Reset_Request_Is_Processed()
    {
        var command = new ForgotPasswordCommand("admin@gammacare.com");

        _authServiceMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Success_Even_When_Email_Does_Not_Exist()
    {
        var command = new ForgotPasswordCommand("missing@gammacare.com");

        _authServiceMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}