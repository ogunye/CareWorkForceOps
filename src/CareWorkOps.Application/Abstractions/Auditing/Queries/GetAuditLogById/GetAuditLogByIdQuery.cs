using CareWorkOps.Application.Auditing.Dtos;
using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Auditing.Queries.GetAuditLogById;

public sealed record GetAuditLogByIdQuery(
    Guid TenantId,
    Guid AuditLogId)
    : IRequest<Result<AuditLogDto>>;