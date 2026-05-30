using System;
using System.Collections.Generic;
using System.Text;

namespace CareWorkOps.Persistence.Identity
{
    public sealed class RefreshToken
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Token { get; private set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime? RevokedAtUtc { get; private set; }

        public bool IsRevoked => RevokedAtUtc.HasValue;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken()
        {
        }

        public RefreshToken(Guid userId, string token, DateTime expiresAtUtc)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            ExpiresAtUtc = expiresAtUtc;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void Revoke()
        {
            if (IsRevoked)
            {
                return;
            }

            RevokedAtUtc = DateTime.UtcNow;
        }
    }
}
