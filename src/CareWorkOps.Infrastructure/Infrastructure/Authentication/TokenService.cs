using CareWorkOps.Application.Abstractions.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareWorkOps.Infrastructure.Infrastructure.Authentication;

public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string CreateAccessToken(TokenUser user)
    {
        ValidateJwtSettings();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("UserId", user.UserId.ToString()),
            new("TenantId", user.TenantId.ToString()),
            new("FullName", user.FullName)
        };

        claims.AddRange(user.Roles.Select(role =>
            new Claim(ClaimTypes.Role, role)));

        claims.AddRange(user.Permissions.Select(permission =>
            new Claim("permission", permission)));

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void ValidateJwtSettings()
    {
        if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is not configured.");

        if (Encoding.UTF8.GetBytes(_jwtSettings.SecretKey).Length < 32)
            throw new InvalidOperationException("JWT SecretKey must be at least 32 bytes / 256 bits.");

        if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured.");

        if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            throw new InvalidOperationException("JWT Audience is not configured.");

        if (_jwtSettings.ExpiryMinutes <= 0)
            throw new InvalidOperationException("JWT ExpiryMinutes must be greater than zero.");
    }
}