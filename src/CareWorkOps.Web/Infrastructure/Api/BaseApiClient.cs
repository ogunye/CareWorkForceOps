using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CareWorkOps.Web.Infrastructure.Api;

public abstract class BaseApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseApiClient(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    protected HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("CareWorkOpsApi");

        var token = _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst("AccessToken")?
            .Value;

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
    {
        var client = CreateClient();

        var response = await client.GetAsync(endpoint);

        return await HandleResponse<T>(response);
    }

    protected async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request)
    {
        var client = CreateClient();

        var json = JsonSerializer.Serialize(request, JsonOptions);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content);

        return await HandleResponse<TResponse>(response);
    }

    protected async Task<ApiResponse<TResponse>> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request)
    {
        var client = CreateClient();

        var json = JsonSerializer.Serialize(request, JsonOptions);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync(endpoint, content);

        return await HandleResponse<TResponse>(response);
    }

    protected async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
    {
        var client = CreateClient();

        var response = await client.DeleteAsync(endpoint);

        return await HandleResponse<bool>(response);
    }

    protected async Task<PagedResult<T>> GetPagedAsync<T>(string endpoint)
    {
        var client = CreateClient();

        var response = await client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiClientException(
                $"API request failed with status code {(int)response.StatusCode}.",
                (int)response.StatusCode);
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<T>>>(
            responseBody,
            JsonOptions);

        var metadata = ExtractPaginationMetadata(response);

        return new PagedResult<T>
        {
            Items = apiResponse?.Data ?? [],
            MetaData = metadata
        };
    }

    private static async Task<ApiResponse<T>> HandleResponse<T>(
        HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return new ApiResponse<T>
            {
                Success = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? "Request completed successfully."
                    : "Request failed."
            };
        }

        var result = JsonSerializer.Deserialize<ApiResponse<T>>(
            responseBody,
            JsonOptions);

        if (result is null)
        {
            throw new ApiClientException(
                "Unable to deserialize API response.",
                (int)response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            result.Success = false;

            if (result.Errors.Count == 0)
            {
                result.Errors.Add(
                    $"API request failed with status code {(int)response.StatusCode}.");
            }
        }

        return result;
    }

    private static MetaData ExtractPaginationMetadata(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("X-Pagination", out var values))
        {
            return new MetaData();
        }

        var paginationHeader = values.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(paginationHeader))
        {
            return new MetaData();
        }

        return JsonSerializer.Deserialize<MetaData>(
            paginationHeader,
            JsonOptions) ?? new MetaData();
    }
}