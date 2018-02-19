using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApi.ErrorHandlingStrategy
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate next;
        private ILogger<LoggingMiddleware> logger;

        public LoggingMiddleware(RequestDelegate next,
        ILogger<LoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Making a back-up of the original response
            Stream originalResponse = context.Response.Body;

            Stream response = new MemoryStream();
            context.Response.Body = response;

            Stopwatch stopWatch = Stopwatch.StartNew();

            LogEntry logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                RequestUri = context.Request.Path,
                RequestMethod = context.Request.Method,
                RequestIpAddress = RequestHelper.GetIp(context),
                RequestThreadId = Thread.CurrentThread.ManagedThreadId,
                RequestContentType = context.Request.ContentType,
                RequestContentBody = await RequestHelper.GetBody(context.Request)
            };

            // Call the next middleware in the pipeline
            await this.next.Invoke(context);

            logEntry.ResponseStatusCode = context.Response.StatusCode;
            logEntry.ResponseContentType = context.Response.ContentType;

            // Logging the response body
            response.Seek(0, SeekOrigin.Begin);
            logEntry.ResponseContentBody = new StreamReader(response).ReadToEnd();

            // Copying the response back to the original one
            response.Seek(0, SeekOrigin.Begin);
            await response.CopyToAsync(originalResponse);

            logEntry.TimeTaken = stopWatch.Elapsed.TotalMilliseconds;

            this.logger.LogInformation(JsonConvert.SerializeObject(logEntry));
        }
    }
}