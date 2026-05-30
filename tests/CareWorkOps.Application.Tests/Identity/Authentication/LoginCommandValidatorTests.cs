using CareWorkOps.Application.Identity.Commands.Authentication;
using FluentValidation.TestHelper;

namespace CareWorkOps.Application.Tests.Identity.Authentication;

public sealed class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = new LoginCommand(
            "admin@alphacare.com",
            "Password123!");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        var command = new LoginCommand(
            email,
            "Password123!");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Have_Error_When_Password_Is_Invalid(string password)
    {
        var command = new LoginCommand(
            "admin@alphacare.com",
            password);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}