using FluentValidation;

namespace CareWorkOps.Application.Auditing.Queries.GetAuditLogById;

public sealed class GetAuditLogByIdQueryValidator : AbstractValidator<GetAuditLogByIdQuery>
{
    public GetAuditLogByIdQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty();

        RuleFor(x => x.AuditLogId)
            .NotEmpty();
    }
}