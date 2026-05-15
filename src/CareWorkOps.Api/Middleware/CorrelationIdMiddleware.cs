namespace CareWorkOps.Api.Middleware
{
    public sealed class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers.TryGetValue(
                CorrelationIdHeaderName,
                out var existingCorrelationId)
                ? existingCorrelationId.ToString()
                : Guid.NewGuid().ToString();

            context.TraceIdentifier = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeaderName] = correlationId;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
