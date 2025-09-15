using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace StudentClub.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log thông tin request
            _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

            // Chỉ log, không chặn authentication
            await _next(context);

            // Log thông tin response
            _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
        }
    }
}
