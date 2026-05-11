using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tenants
{
    public enum TenantIsolationMode
    {
        SharedDatabase = 1,
        SchemaPerTenant = 2,
        DedicatedDatabase = 3
    }
}
