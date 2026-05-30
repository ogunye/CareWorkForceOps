using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Identity
{
    public enum PermissionGroup
    {
        TenantAdministration = 1,
        UserManagement = 2,
        RoleManagement = 3,
        StaffManagement = 4,
        ServiceUsers = 5,
        CarePlans = 6,
        RotaScheduling = 7,
        Medication = 8,
        IncidentReporting = 9,
        Compliance = 10,
        Billing = 11,
        Audit = 12
    }
}
