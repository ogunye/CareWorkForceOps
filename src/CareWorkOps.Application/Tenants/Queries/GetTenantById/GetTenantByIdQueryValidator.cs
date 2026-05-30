using FluentValidation;

namespace CareWorkOps.Application.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdQueryValidator : AbstractValidator<GetTenantByIdQuery>
{
    public GetTenantByIdQueryValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
    }
}