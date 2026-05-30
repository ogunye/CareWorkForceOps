using CareWorkOps.Domain.Identity;
using FluentValidation;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.AssignPermissionsToRole;

public sealed class AssignPermissionsToRoleCommandValidator
    : AbstractValidator<AssignPermissionsToRoleCommand>
{
    public AssignPermissionsToRoleCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();

        RuleFor(x => x.Permissions)
            .NotNull();

        RuleForEach(x => x.Permissions)
            .Must(PermissionCatalogue.IsValid)
            .WithMessage("Invalid permission code.");
    }
}