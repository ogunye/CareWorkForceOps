using CareWorkOps.Domain.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tenants.Commands.CreateTenant
{
    public sealed record CreateTenantResponse(
     Guid TenantId,
     string TenantName,
     string TenantSlug,
     TenantIsolationMode IsolationMode,
     TenantStatus Status,
     Guid AdminUserId);
}
