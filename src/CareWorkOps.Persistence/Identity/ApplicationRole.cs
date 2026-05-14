using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Persistence.Identity
{
    public sealed class ApplicationRole : IdentityRole<Guid>
    {
        public Guid? TenantId { get; private set; }

        private ApplicationRole()
        {
        }

        public ApplicationRole(string name, Guid? tenantId = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            NormalizedName = name.ToUpperInvariant();
            TenantId = tenantId;
        }
    }
}
