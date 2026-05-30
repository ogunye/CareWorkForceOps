using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CareWorkOps.Integration.Tests.Infrastructure;

public static class TestAuthHelper
{
    public static async Task<string> LoginAndGetAccessTokenAsync(
        HttpClient client,
        string email,
        string password)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new
            {
                email,
                password
            });

        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Login failed. StatusCode: {(int)response.StatusCode}. Body: {body}");
        }

        using var document = JsonDocument.Parse(body);

        return document.RootElement
            .GetProperty("data")
            .GetProperty("accessToken")
            .GetString()!;
    }

    public static void Authorize(HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public static void ClearAuthorization(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}