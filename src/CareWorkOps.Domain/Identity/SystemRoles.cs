using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Identity
{
    public static class SystemRoles
    {
        public const string PlatformAdmin = "PlatformAdmin";
        public const string TenantAdmin = "TenantAdmin";
        public const string CareManager = "CareManager";
        public const string CareCoordinator = "CareCoordinator";
        public const string Carer = "Carer";
        public const string Auditor = "Auditor";
        public const string FinanceOfficer = "FinanceOfficer";
    }
}
