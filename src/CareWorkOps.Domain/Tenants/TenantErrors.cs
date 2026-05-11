using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Domain.Tenants
{
    public static class TenantErrors
    {
        public const string NameRequired = "Tenant name is required.";
        public const string SlugRequired = "Tenant slug is required.";
        public const string InvalidSlug = "Tenant slug can only contain lowercase letters, numbers, and hyphens.";
        public const string AlreadySuspended = "Tenant is already suspended.";
        public const string AlreadyActive = "Tenant is already active.";
        public const string ArchivedTenantCannotBeModified = "Archived tenant cannot be modified.";
        public const string DedicatedDatabaseRequiresConnectionString = "Dedicated database tenants require a connection string.";
    }
}
