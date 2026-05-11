using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOnUtc { get; }
    }
}
