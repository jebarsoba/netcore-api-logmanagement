using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TodoApi.ErrorHandlingStrategy
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next,
        ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            LoggingMiddlewareLogEntry logEntry = new LoggingMiddlewareLogEntry
            {
                Timestamp = DateTime.Now
            };

            _logger.LogInformation("Request received {0}", logEntry);

            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }
    }
}