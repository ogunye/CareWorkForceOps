using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.ArchiveTenant;

public sealed record ArchiveTenantCommand(Guid TenantId) : IRequest<Result>;