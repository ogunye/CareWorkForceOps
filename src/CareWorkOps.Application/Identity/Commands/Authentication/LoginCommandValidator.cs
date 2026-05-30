using FluentValidation;

namespace CareWorkOps.Application.Identity.Commands.Authentication
{
    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(250);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}
