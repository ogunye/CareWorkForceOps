using CareWorkOps.Domain.Common;

namespace CareWorkOps.Domain.Tenants.Events
{
    public sealed record TenantCreatedDomainEvent(
    Guid TenantId,
    string Name,
    string Slug,
    DateTime OccurredOnUtc) : IDomainEvent;
}
