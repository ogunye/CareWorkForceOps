using FluentValidation;

namespace CareWorkOps.Application.Tenants.Commands.ArchiveTenant;

public sealed class ArchiveTenantCommandValidator : AbstractValidator<ArchiveTenantCommand>
{
    public ArchiveTenantCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}