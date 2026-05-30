using FluentValidation;

namespace CareWorkOps.Application.Auditing.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty();
    }
}