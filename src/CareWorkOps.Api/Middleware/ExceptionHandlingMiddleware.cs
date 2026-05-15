using CareWorkOps.Api.Common;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CareWorkOps.Api.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException exception)
            {
                await HandleExceptionAsync(
                    context,
                    exception,
                    HttpStatusCode.BadRequest,
                    "Validation failed.",
                    exception.Errors.Select(x => x.ErrorMessage).ToArray());
            }
            catch (UnauthorizedAccessException exception)
            {
                await HandleExceptionAsync(
                    context,
                    exception,
                    HttpStatusCode.Unauthorized,
                    "Unauthorized request.",
                    ["You are not authorized to perform this action."]);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(
                    context,
                    exception,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.",
                    ["An unexpected error occurred. Please contact support."]);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            HttpStatusCode statusCode,
            string message,
            IReadOnlyCollection<string> errors)
        {
            _logger.LogError(
                exception,
                "Request failed. CorrelationId: {CorrelationId}, Path: {Path}",
                context.TraceIdentifier,
                context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Fail(
                message,
                errors,
                context.TraceIdentifier);

            var json = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await context.Response.WriteAsync(json);
        }
    }
}
