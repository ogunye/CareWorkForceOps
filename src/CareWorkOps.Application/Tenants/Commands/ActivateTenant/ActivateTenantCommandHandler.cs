using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Common;
using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Tenants;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.ActivateTenant;

public sealed class ActivateTenantCommandHandler : IRequestHandler<ActivateTenantCommand, Result>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public ActivateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result> Handle(
        ActivateTenantCommand request,
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
            tenant.Activate();
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation(ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditService.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantActivated",
            "Tenant",
            tenant.Id.Value.ToString(),
            $"Tenant '{tenant.Name}' was activated.",
            cancellationToken);

        return Result.Success();
    }
}