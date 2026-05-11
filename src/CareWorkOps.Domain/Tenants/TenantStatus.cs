using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tenants
{
    public enum TenantStatus
    {
        Active = 1,
        Suspended = 2,
        PendingSetup = 3,
        Archived = 4
    }
}
