using CareWorkOps.Domain.Common;
using CareWorkOps.Domain.Tenants.Events;
using System.Text.RegularExpressions;

namespace CareWorkOps.Domain.Tenants
{
    public sealed class Tenant : AuditableEntity<TenantId>
    {
        private static readonly Regex SlugRegex =
            new("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);

        public string Name { get; private set; } = string.Empty;

        public string Slug { get; private set; } = string.Empty;

        public TenantIsolationMode IsolationMode { get; private set; }

        public TenantStatus Status { get; private set; }

        public string? ConnectionString { get; private set; }

        private Tenant()
        {
        }

        private Tenant(
            TenantId id,
            string name,
            string slug,
            TenantIsolationMode isolationMode,
            string? connectionString)
            : base(id, id.Value)
        {
            Name = ValidateName(name);
            Slug = ValidateSlug(slug);
            IsolationMode = isolationMode;
            ConnectionString = ValidateConnectionString(isolationMode, connectionString);
            Status = TenantStatus.PendingSetup;

            RaiseDomainEvent(new TenantCreatedDomainEvent(
                Id.Value,
                Name,
                Slug,
                DateTime.UtcNow));
        }

        public static Tenant Create(
            string name,
            string slug,
            TenantIsolationMode isolationMode,
            string? connectionString = null)
        {
            return new Tenant(
                Domain.Tenants.TenantId.New(), // Fully qualify to avoid ambiguity with AuditableEntity<TenantId>.TenantId
                name,
                slug,
                isolationMode,
                connectionString);
        }

        public void Activate()
        {
            EnsureNotArchived();

            if (Status == TenantStatus.Active)
            {
                throw new DomainException(TenantErrors.AlreadyActive);
            }

            Status = TenantStatus.Active;

            RaiseDomainEvent(new TenantActivatedDomainEvent(
                Id.Value,
                DateTime.UtcNow));
        }

        public void Suspend(string reason)
        {
            EnsureNotArchived();

            if (Status == TenantStatus.Suspended)
            {
                throw new DomainException(TenantErrors.AlreadySuspended);
            }

            var suspensionReason = Guard.AgainstNullOrWhiteSpace(
                reason,
                nameof(reason),
                500);

            Status = TenantStatus.Suspended;

            RaiseDomainEvent(new TenantSuspendedDomainEvent(
                Id.Value,
                suspensionReason,
                DateTime.UtcNow));
        }

        public void Archive()
        {
            Status = TenantStatus.Archived;
        }

        public void Rename(string name)
        {
            EnsureNotArchived();

            Name = ValidateName(name);
        }

        public void ChangeIsolationMode(
            TenantIsolationMode isolationMode,
            string? connectionString = null)
        {
            EnsureNotArchived();

            IsolationMode = isolationMode;
            ConnectionString = ValidateConnectionString(isolationMode, connectionString);
        }

        private static string ValidateName(string name)
        {
            return Guard.AgainstNullOrWhiteSpace(name, nameof(name), 150);
        }

        private static string ValidateSlug(string slug)
        {
            slug = Guard.AgainstNullOrWhiteSpace(slug, nameof(slug), 100)
                .ToLowerInvariant();

            if (!SlugRegex.IsMatch(slug))
            {
                throw new DomainException(TenantErrors.InvalidSlug);
            }

            return slug;
        }

        private static string? ValidateConnectionString(
            TenantIsolationMode isolationMode,
            string? connectionString)
        {
            if (isolationMode == TenantIsolationMode.DedicatedDatabase &&
                string.IsNullOrWhiteSpace(connectionString))
            {
                throw new DomainException(TenantErrors.DedicatedDatabaseRequiresConnectionString);
            }

            return string.IsNullOrWhiteSpace(connectionString)
                ? null
                : connectionString.Trim();
        }

        private void EnsureNotArchived()
        {
            if (Status == TenantStatus.Archived)
            {
                throw new DomainException(TenantErrors.ArchivedTenantCannotBeModified);
            }
        }
    }
}
