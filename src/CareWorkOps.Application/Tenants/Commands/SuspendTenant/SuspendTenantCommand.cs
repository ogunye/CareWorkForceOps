using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.SuspendTenant;

public sealed record SuspendTenantCommand(
    Guid TenantId,
    string Reason)
    : IRequest<Result>;