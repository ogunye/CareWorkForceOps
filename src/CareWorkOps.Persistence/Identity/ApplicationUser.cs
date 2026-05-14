using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Persistence.Identity
{
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        public Guid TenantId { get; private set; }

        public string FirstName { get; private set; } = string.Empty;

        public string LastName { get; private set; } = string.Empty;

        public bool IsActive { get; private set; } = true;

        private ApplicationUser()
        {
        }

        public ApplicationUser(
            Guid tenantId,
            string firstName,
            string lastName,
            string email)
        {
            Id = Guid.NewGuid();
            TenantId = tenantId;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            UserName = Email;
            NormalizedEmail = Email.ToUpperInvariant();
            NormalizedUserName = Email.ToUpperInvariant();
            EmailConfirmed = true;
            IsActive = true;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
