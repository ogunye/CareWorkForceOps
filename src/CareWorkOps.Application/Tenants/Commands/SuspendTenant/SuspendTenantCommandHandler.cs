using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Common;
using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Tenants;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.SuspendTenant;

public sealed class SuspendTenantCommandHandler : IRequestHandler<SuspendTenantCommand, Result>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public SuspendTenantCommandHandler(
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result> Handle(
        SuspendTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            TenantId.From(request.TenantId),
            cancellationToken);

        if (tenant is null)
        {
            return Result.Failure(Error.Failure("Tenant was not found."));
        }

        try
        {
            tenant.Suspend(request.Reason);
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation(ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditService.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantSuspended",
            "Tenant",
            tenant.Id.Value.ToString(),
            $"Tenant '{tenant.Name}' was suspended. Reason: {request.Reason}",
            cancellationToken);

        return Result.Success();
    }
}