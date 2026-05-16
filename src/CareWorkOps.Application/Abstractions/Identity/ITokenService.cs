using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Abstractions.Identity
{
    public interface ITokenService
    {
        string CreateAccessToken(TokenUser user);
    }

    public sealed record TokenUser(
    Guid UserId,
    Guid TenantId,
    string Email,
    string FullName,
    IReadOnlyCollection<string> Roles);
}
