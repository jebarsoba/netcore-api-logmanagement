using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
            _logger.LogInformation("Logging request.");

            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }
    }
}
