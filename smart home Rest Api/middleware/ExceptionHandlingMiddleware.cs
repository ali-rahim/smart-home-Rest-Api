namespace smart_home_Asp.net
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using smart_home_Asp.net.Exceptions;

    namespace YourProjectName.Middleware
    {
        public class ExceptionHandlingMiddleware
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
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            }

            private async Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                var (statusCode, title) = exception switch
                {
                    EntityNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                    EntityAlreadyExistsException => (StatusCodes.Status409Conflict, "Conflict"),
                    InvalidChildException => (StatusCodes.Status400BadRequest, "Bad Request"),
                    ArgumentNullException => (StatusCodes.Status400BadRequest, "Bad Request"),  // اول نوع خاص‌تر
                    ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),      // بعد نوع کلی‌تر
                    InvalidOperationException => (StatusCodes.Status400BadRequest, "Bad Request"),
                    _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
                };

                _logger.LogError(
                    exception,
                    "Unhandled exception occurred. Method={Method}, Path={Path}, StatusCode={StatusCode}, Error={Error}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    exception.Message);

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = exception.Message
                });
            }
        }

        public static class ExceptionHandlingMiddlewareExtensions
        {
            public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<ExceptionHandlingMiddleware>();
            }
        }
    }
}