using CareWorkOps.Application.Abstractions.Identity;
using CareWorkOps.Application.Abstractions.Persistence;
using CareWorkOps.Application.Common;
using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Tenants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tenants.Commands.CreateTenant
{
    public sealed class CreateTenantCommandHandler
    : IRequestHandler<CreateTenantCommand, Result<CreateTenantResponse>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTenantCommandHandler(
            ITenantRepository tenantRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateTenantResponse>> Handle(
            CreateTenantCommand request,
            CancellationToken cancellationToken)
        {
            var normalizedSlug = request.TenantSlug.Trim().ToLowerInvariant();

            var tenantExists = await _tenantRepository.ExistsBySlugAsync(
                normalizedSlug,
                cancellationToken);

            if (tenantExists)
            {
                return Result<CreateTenantResponse>.Failure(
                    Error.Conflict($"Tenant with slug '{normalizedSlug}' already exists."));
            }

            if (!Enum.TryParse<TenantIsolationMode>(
                    request.IsolationMode,
                    ignoreCase: false,
                    out var isolationMode))
            {
                return Result<CreateTenantResponse>.Failure(
                    Error.Validation("Invalid tenant isolation mode."));
            }

            Tenant tenant;

            try
            {
                tenant = Tenant.Create(
                    request.TenantName,
                    normalizedSlug,
                    isolationMode,
                    request.ConnectionString);
            }
            catch (DomainException exception)
            {
                return Result<CreateTenantResponse>.Failure(
                    Error.Validation(exception.Message));
            }

            await _tenantRepository.AddAsync(tenant, cancellationToken);

            var identityResult = await _identityService.CreateTenantAdminAsync(
                tenant.Id.Value,
                request.AdminFirstName,
                request.AdminLastName,
                request.AdminEmail,
                request.AdminPassword,
                cancellationToken);

            if (!identityResult.Succeeded || identityResult.UserId is null)
            {
                return Result<CreateTenantResponse>.Failure(
                    Error.Failure(string.Join("; ", identityResult.Errors)));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateTenantResponse(
                tenant.Id.Value,
                tenant.Name,
                tenant.Slug,
                tenant.IsolationMode,
                tenant.Status,
                identityResult.UserId.Value);

            return Result<CreateTenantResponse>.Success(response);
        }
    }
}
