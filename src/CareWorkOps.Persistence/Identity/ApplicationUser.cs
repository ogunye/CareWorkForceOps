using CareWorkOps.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CareWorkOps.Persistence.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public UserStatus Status { get; private set; } = UserStatus.Active;

    public bool IsActive => Status == UserStatus.Active;

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
        Status = UserStatus.Active;
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    public void Reactivate()
    {
        Status = UserStatus.Active;
    }
}