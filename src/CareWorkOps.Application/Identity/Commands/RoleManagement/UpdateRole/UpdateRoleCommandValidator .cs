using FluentValidation;

namespace CareWorkOps.Application.Identity.Commands.RoleManagement.UpdateRole;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(100);
    }
}