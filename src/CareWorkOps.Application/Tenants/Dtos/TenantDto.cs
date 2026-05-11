using CareWorkOps.Domain.Tenants;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tenants.Dtos
{
    public sealed record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    TenantIsolationMode IsolationMode,
    TenantStatus Status);
}
