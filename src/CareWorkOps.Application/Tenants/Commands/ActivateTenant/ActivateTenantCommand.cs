using CareWorkOps.Application.Common;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.ActivateTenant;

public sealed record ActivateTenantCommand(Guid TenantId) : IRequest<Result>;