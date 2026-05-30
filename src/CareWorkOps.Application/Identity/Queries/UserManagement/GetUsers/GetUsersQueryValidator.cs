using FluentValidation;

namespace CareWorkOps.Application.Identity.Queries.UserManagement.GetUsers;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}