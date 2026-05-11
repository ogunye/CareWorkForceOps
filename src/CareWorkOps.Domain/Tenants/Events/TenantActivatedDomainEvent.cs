using CareWorkOps.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tenants.Events
{
    public sealed record TenantActivatedDomainEvent(
    Guid TenantId,
    DateTime OccurredOnUtc) : IDomainEvent;
}
