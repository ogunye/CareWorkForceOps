using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Auditing.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, Result<IReadOnlyCollection<AuditLogDto>>>
{
    private readonly IAuditLogService _auditLogService;

    public GetAuditLogsQueryHandler(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task<Result<IReadOnlyCollection<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var auditLogs = await _auditLogService.GetAuditLogsAsync(
            request.TenantId,
            cancellationToken);

        return Result<IReadOnlyCollection<AuditLogDto>>.Success(auditLogs);
    }
}