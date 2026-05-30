using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Auditing.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    Guid TenantId)
    : IRequest<Result<IReadOnlyCollection<AuditLogDto>>>;