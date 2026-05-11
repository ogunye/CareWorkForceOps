using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tenants.Commands.CreateTenant
{
    public sealed class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(x => x.TenantName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.TenantSlug)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
                .WithMessage("Tenant slug can only contain lowercase letters, numbers, and hyphens.");

            RuleFor(x => x.AdminFirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.AdminLastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.AdminEmail)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(250);

            RuleFor(x => x.AdminPassword)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.IsolationMode)
                .NotEmpty()
                .Must(BeValidIsolationMode)
                .WithMessage("Invalid tenant isolation mode.");

            RuleFor(x => x.ConnectionString)
                .NotEmpty()
                .When(x => x.IsolationMode == "DedicatedDatabase")
                .WithMessage("Connection string is required for dedicated database tenants.");
        }

        private static bool BeValidIsolationMode(string isolationMode)
        {
            return isolationMode is
                "SharedDatabase" or
                "SchemaPerTenant" or
                "DedicatedDatabase";
        }
    }
}
