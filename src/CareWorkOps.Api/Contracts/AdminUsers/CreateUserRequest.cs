namespace CareWorkOps.Api.Contracts.AdminUsers;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IReadOnlyCollection<string> Roles);