using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Common
{
    public abstract class AuditableEntity<TId> : AggregateRoot<TId>
     where TId : notnull
    {
        public Guid? TenantId { get; protected set; }

        public DateTime CreatedAtUtc { get; protected set; }

        public Guid? CreatedByUserId { get; protected set; }

        public DateTime? UpdatedAtUtc { get; protected set; }

        public Guid? UpdatedByUserId { get; protected set; }

        public DateTime? DeletedAtUtc { get; protected set; }

        public Guid? DeletedByUserId { get; protected set; }

        public bool IsDeleted { get; protected set; }

        public byte[] RowVersion { get; protected set; } = [];

        protected AuditableEntity()
        {
        }

        protected AuditableEntity(TId id, Guid? tenantId = null)
            : base(id)
        {
            TenantId = tenantId;
            CreatedAtUtc = DateTime.UtcNow;
            IsDeleted = false;
        }

        public void MarkAsCreated(Guid? userId, DateTime utcNow)
        {
            CreatedByUserId = userId;
            CreatedAtUtc = utcNow;
        }

        public void MarkAsUpdated(Guid? userId, DateTime utcNow)
        {
            UpdatedByUserId = userId;
            UpdatedAtUtc = utcNow;
        }

        public void MarkAsDeleted(Guid? userId, DateTime utcNow)
        {
            if (IsDeleted)
            {
                return;
            }

            IsDeleted = true;
            DeletedByUserId = userId;
            DeletedAtUtc = utcNow;
        }
    }
}
