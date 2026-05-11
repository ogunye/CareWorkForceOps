using CareWorkOps.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tenants
{
    public readonly record struct TenantId(Guid Value)
    {
        public static TenantId New()
        {
            return new TenantId(Guid.NewGuid());
        }

        public static TenantId From(Guid value)
        {
            Guard.AgainstEmptyGuid(value, nameof(TenantId));
            return new TenantId(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
