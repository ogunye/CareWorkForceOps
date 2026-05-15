using CareWorkOps.Api.Middleware;

namespace CareWorkOps.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseGlobalExceptionHandling(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static IApplicationBuilder UseStructuredRequestLogging(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
