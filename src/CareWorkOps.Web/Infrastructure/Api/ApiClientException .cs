namespace CareWorkOps.Web.Infrastructure.Api;

public sealed class ApiClientException : Exception
{
    public int? StatusCode { get; }

    public ApiClientException(string message, int? statusCode = null)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public ApiClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}