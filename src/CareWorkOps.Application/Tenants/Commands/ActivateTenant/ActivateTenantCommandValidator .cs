using FluentValidation;

namespace CareWorkOps.Application.Tenants.Commands.ActivateTenant;

public sealed class ActivateTenantCommandValidator : AbstractValidator<ActivateTenantCommand>
{
    public ActivateTenantCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}