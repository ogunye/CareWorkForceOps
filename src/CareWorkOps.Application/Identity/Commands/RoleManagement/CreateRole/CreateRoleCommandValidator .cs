using CareWorkOps.Domain.Identity;
using FluentValidation;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .MaximumLength(100);

        RuleForEach(x => x.Permissions)
            .Must(PermissionCatalogue.IsValid)
            .WithMessage("Invalid permission code.");
    }
}