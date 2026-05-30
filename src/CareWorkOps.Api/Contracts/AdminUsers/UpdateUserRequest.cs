namespace CareWorkOps.Api.Contracts.AdminUsers;

public sealed record UpdateUserRequest(
    string FirstName,
    string LastName);