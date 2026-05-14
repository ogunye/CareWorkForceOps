namespace CareWorkOps.Api.Common
{
    public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyCollection<string> Errors,
    string? CorrelationId)
    {
        public static ApiResponse<T> Ok(
            T data,
            string message,
            string? correlationId)
        {
            return new ApiResponse<T>(
                true,
                message,
                data,
                Array.Empty<string>(),
                correlationId);
        }

        public static ApiResponse<T> Fail(
            string message,
            IReadOnlyCollection<string> errors,
            string? correlationId)
        {
            return new ApiResponse<T>(
                false,
                message,
                default,
                errors,
                correlationId);
        }
    }
}
