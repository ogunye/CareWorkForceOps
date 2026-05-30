using FluentValidation;

namespace CareWorkOps.Application.Identity.Queries.RoleManagement.GetRoles;

public sealed class GetRolesQueryValidator : AbstractValidator<GetRolesQuery>
{
    public GetRolesQueryValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}