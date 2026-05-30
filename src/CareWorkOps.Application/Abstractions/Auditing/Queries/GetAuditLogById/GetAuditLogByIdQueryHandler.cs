using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Auditing.Queries.GetAuditLogById;

public sealed class GetAuditLogByIdQueryHandler
    : IRequestHandler<GetAuditLogByIdQuery, Result<AuditLogDto>>
{
    private readonly IAuditLogService _auditLogService;

    public GetAuditLogByIdQueryHandler(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task<Result<AuditLogDto>> Handle(
        GetAuditLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var auditLog = await _auditLogService.GetAuditLogByIdAsync(
            request.TenantId,
            request.AuditLogId,
            cancellationToken);

        return auditLog is null
            ? Result<AuditLogDto>.Failure(Error.Failure("Audit log was not found."))
            : Result<AuditLogDto>.Success(auditLog);
    }
}