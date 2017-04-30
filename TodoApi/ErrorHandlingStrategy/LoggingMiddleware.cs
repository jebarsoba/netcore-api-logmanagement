using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
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

        public async Task Invoke(HttpContext context)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            LoggingMiddlewareLogEntry logEntry = new LoggingMiddlewareLogEntry
            {
                Timestamp = DateTime.Now,
                RequestUri = context.Request.Path,
                RequestMethod = context.Request.Method,
                RequestThreadId = Thread.CurrentThread.ManagedThreadId,
                RequestIpAddress = this.GetRequestIpAddress(context),
                RequestContentType = context.Request.ContentType
            };

            // Call the next delegate/middleware in the pipeline
            await _next.Invoke(context);

            logEntry.ResponseStatusCode = context.Response.StatusCode;
            logEntry.TimeTaken = stopWatch.Elapsed.TotalMilliseconds;
            logEntry.ResponseContentType = context.Response.ContentType;

            _logger.LogInformation(JsonConvert.SerializeObject(logEntry));
        }

        private string GetRequestIpAddress(HttpContext context)
        {
            IHttpConnectionFeature connection = context.Features.Get<IHttpConnectionFeature>();

            return connection?.RemoteIpAddress?.ToString();
        }
    }
}