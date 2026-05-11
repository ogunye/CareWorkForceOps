using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Common
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
    {
        protected AggregateRoot()
        {
        }

        protected AggregateRoot(TId id)
            : base(id)
        {
        }
    }
}
