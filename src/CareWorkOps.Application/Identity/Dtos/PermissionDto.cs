using CareWorkOps.Domain.Identity;

namespace CareWorkOps.Application.Identity.Dtos;

public sealed record PermissionDto(
    string Code,
    string Name,
    PermissionGroup Group,
    string Description);