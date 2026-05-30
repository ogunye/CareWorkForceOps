using CareWorkOps.Application.Abstractions.Auditing;
using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Common;
using CareWorkOps.Application.Tenants.Dtos;
using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Tenants;
using MediatR;

namespace CareWorkOps.Application.Tenants.Commands.UpdateTenant;

public sealed class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork,
        IAuditService auditService)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Result<TenantDto>> Handle(
        UpdateTenantCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            TenantId.From(request.TenantId),
            cancellationToken);

        if (tenant is null)
        {
            return Result<TenantDto>.Failure(Error.Failure("Tenant was not found."));
        }

        if (!Enum.TryParse<TenantIsolationMode>(request.IsolationMode, out var isolationMode))
        {
            return Result<TenantDto>.Failure(Error.Validation("Invalid tenant isolation mode."));
        }

        try
        {
            tenant.Rename(request.Name);
            tenant.ChangeIsolationMode(isolationMode, request.ConnectionString);
        }
        catch (DomainException ex)
        {
            return Result<TenantDto>.Failure(Error.Validation(ex.Message));
        }

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditService.RecordAsync(
            tenant.Id.Value,
            null,
            "TenantUpdated",
            "Tenant",
            tenant.Id.Value.ToString(),
            $"Tenant '{tenant.Name}' was updated.",
            cancellationToken);

        return Result<TenantDto>.Success(Map(tenant));
    }

    private static TenantDto Map(Tenant tenant)
    {
        return new TenantDto(
            tenant.Id.Value,
            tenant.Name,
            tenant.Slug,
            tenant.IsolationMode,
            tenant.Status,
            tenant.ConnectionString);
    }
}