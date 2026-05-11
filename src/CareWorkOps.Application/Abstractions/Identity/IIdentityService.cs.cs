using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Application.Abstractions.Identity
{
    public interface IIdentityService
    {
        Task<CreateUserResult> CreateTenantAdminAsync(
            Guid tenantId,
            string firstName,
            string lastName,
            string email,
            string password,
            CancellationToken cancellationToken = default);
    }

    public sealed record CreateUserResult(
        bool Succeeded,
        Guid? UserId,
        IReadOnlyCollection<string> Errors)
    {
        public static CreateUserResult Success(Guid userId)
        {
            return new CreateUserResult(true, userId, Array.Empty<string>());
        }

        public static CreateUserResult Failure(IEnumerable<string> errors)
        {
            return new CreateUserResult(false, null, errors.ToArray());
        }
    }
}
