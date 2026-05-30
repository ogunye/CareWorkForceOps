using FluentValidation;

namespace CareWorkOps.Application.Tenants.Commands.UpdateTenant;

public sealed class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

        RuleFor(x => x.IsolationMode)
            .NotEmpty()
            .Must(x => x is "SharedDatabase" or "SchemaPerTenant" or "DedicatedDatabase");

        RuleFor(x => x.ConnectionString)
            .NotEmpty()
            .When(x => x.IsolationMode == "DedicatedDatabase");
    }
}