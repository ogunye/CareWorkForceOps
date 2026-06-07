using CareWorkOps.Domain.Identity;

namespace CareWorkOps.Api.IntegrationTests.Infrastructure;

public static class TestClientExtensions
{
    public static void AuthorizeAsTenantAdmin(
        this HttpClient client,
        params string[] permissions)
    {
        client.DefaultRequestHeaders.Remove("X-Test-Auth");
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");

        client.DefaultRequestHeaders.Remove("X-Test-Permissions");

        if (permissions.Length > 0)
        {
            client.DefaultRequestHeaders.Add("X-Test-Permissions", string.Join(",", permissions));
        }
        else
        {
            client.DefaultRequestHeaders.Add("X-Test-Permissions", SystemPermissions.AuditView);
        }
    }

    public static void ClearAuthorization(this HttpClient client)
    {
        client.DefaultRequestHeaders.Remove("X-Test-Auth");
        client.DefaultRequestHeaders.Remove("X-Test-Permissions");
        client.DefaultRequestHeaders.Authorization = null;
    }
}