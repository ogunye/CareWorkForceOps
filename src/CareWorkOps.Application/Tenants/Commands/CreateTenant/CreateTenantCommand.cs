using CareWorkOps.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Tenants.Commands.CreateTenant
{
    public sealed record CreateTenantCommand(
    string TenantName,
    string TenantSlug,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword,
    string IsolationMode,
    string? ConnectionString = null)
    : IRequest<Result<CreateTenantResponse>>;
}
