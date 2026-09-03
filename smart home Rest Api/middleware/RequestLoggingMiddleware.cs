using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace smart_home_Asp.net
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next , ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger= logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            const string correlationHeader = "X-Correlation-ID";
            var correlationId = httpContext.Request.Headers[correlationHeader].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString("N");
            }
            httpContext.Response.Headers["X-Correlation-ID"] = correlationId;

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            });


            _logger.LogInformation("Request started. Method={Method}, Path={Path}",
                httpContext.Request.Method, httpContext.Request.Path);



            var stopwatch = Stopwatch.StartNew();
            await _next(httpContext);
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;


            _logger.LogInformation("Request completed. Method={Method}, Path={Path}, StatusCode={StatusCode}, ElapsedMs={ElapsedMs}", httpContext.Request.Method, httpContext.Request.Path , httpContext.Response.StatusCode, elapsedMilliseconds );



        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
