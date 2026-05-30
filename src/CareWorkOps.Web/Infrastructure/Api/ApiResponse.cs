namespace CareWorkOps.Web.Infrastructure.Api;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public T? Data { get; set; }

    public List<string> Errors { get; set; } = [];
}