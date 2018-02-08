﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
            //Taking a copy of the original response body (to copy it back at the end)
            var bodyStream = context.Response.Body;

            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            Stopwatch stopWatch = Stopwatch.StartNew();

            LogEntry logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                RequestUri = context.Request.Path,
                RequestMethod = context.Request.Method,
                RequestIpAddress = this.GetRequestIpAddress(context),
                RequestThreadId = Thread.CurrentThread.ManagedThreadId,
                RequestContentType = context.Request.ContentType,
                RequestContentBody = await this.GetRequestBody(context.Request)
            };

            // Call the next delegate/middleware in the pipeline
            await _next.Invoke(context);

            logEntry.ResponseStatusCode = context.Response.StatusCode;
            logEntry.ResponseContentType = context.Response.ContentType;

            //Logging the response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
            logEntry.ResponseContentBody = responseBody;

            //Copying back the original response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(bodyStream);

            logEntry.TimeTaken = stopWatch.Elapsed.TotalMilliseconds;

            _logger.LogInformation(JsonConvert.SerializeObject(logEntry));
        }

        private string GetRequestIpAddress(HttpContext context)
        {
            IHttpConnectionFeature connection = context.Features.Get<IHttpConnectionFeature>();

            return connection?.RemoteIpAddress?.ToString();
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableRewind();

            byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);

            request.Body.Seek(0, SeekOrigin.Begin);

            return requestBody;
        }
    }
}