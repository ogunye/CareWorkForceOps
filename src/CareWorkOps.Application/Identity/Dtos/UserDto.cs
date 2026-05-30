using CareWorkOps.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Identity.Dtos
{
    public sealed record UserDto(
     Guid Id,
     Guid TenantId,
     string FirstName,
     string LastName,
     string Email,
     UserStatus Status,
     IReadOnlyCollection<string> Roles);
}
