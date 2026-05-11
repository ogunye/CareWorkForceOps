using CareWorkOps.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tenants.Events
{
    public sealed record TenantSuspendedDomainEvent(
    Guid TenantId,
    string Reason,
    DateTime OccurredOnUtc) : IDomainEvent;
}
