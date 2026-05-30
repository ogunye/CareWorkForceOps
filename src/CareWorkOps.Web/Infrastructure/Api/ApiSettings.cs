namespace CareWorkOps.Web.Infrastructure.Api;

public sealed class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 60;
}