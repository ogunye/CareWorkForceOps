namespace CareWorkOps.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    Guid? TenantId { get; }

    string? Email { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsAuthenticated { get; }
}