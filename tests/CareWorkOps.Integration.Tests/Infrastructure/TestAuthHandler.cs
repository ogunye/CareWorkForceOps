using CareWorkOps.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CareWorkOps.Api.IntegrationTests.Infrastructure;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";
    public const string HeaderName = "X-Test-Auth";
    public const string PermissionsHeaderName = "X-Test-Permissions";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(HeaderName))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, TestClaimsProvider.UserId),
        new(ClaimTypes.Email, TestClaimsProvider.Email),

        // Required by [Authorize(Policy = "TenantAdminOnly")]
        new(ClaimTypes.Role, "TenantAdmin"),
        new("role", "TenantAdmin"),

        // Required by AdminAuditLogsController.GetTenantId()
        new("TenantId", TestClaimsProvider.TenantId)
    };

        var requestedPermissions =
            Request.Headers[PermissionsHeaderName].ToString();

        if (!string.IsNullOrWhiteSpace(requestedPermissions))
        {
            foreach (var permission in requestedPermissions.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                claims.Add(new Claim("Permission", permission));
            }
        }
        else
        {
            claims.Add(new Claim("Permission", SystemPermissions.AuditView));
        }

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}