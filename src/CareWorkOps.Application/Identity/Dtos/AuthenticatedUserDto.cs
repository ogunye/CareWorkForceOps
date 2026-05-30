namespace CareWorkOps.Application.Identity.Dtos;

public sealed record AuthenticatedUserDto(
    Guid UserId,
    Guid TenantId,
    string Email,
    string FullName,
    IReadOnlyCollection<string> Roles,
    string AccessToken,
    string RefreshToken);