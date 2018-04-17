using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApi.ErrorHandlingStrategy
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        private readonly int EXCEPTION_MSG_LOG_BYTES_MAX_LENGHT = 1024;

        public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ExceptionLogEntry errorLogEntry = new ExceptionLogEntry()
            {
                Timestamp = DateTime.Now,
                User = RequestHelper.GetUser(context.Request),
                RequestUri = context.Request.Path,
                RequestMethod = context.Request.Method,
                ExceptionType = exception.GetType().ToString(),
                ExceptionMessage = exception.Message,
                RequestIpAddress = RequestHelper.GetIp(context),
                RequestThreadId = Thread.CurrentThread.ManagedThreadId,
                RequestContentType = context.Request.ContentType,
                RequestContentBody = await RequestHelper.GetBody(context),
                ExceptionFullMessage = this.GetSanitizedExceptionMessage(exception)
            };

            this.logger.LogError(JsonConvert.SerializeObject(errorLogEntry));

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync("{ \"ErrorMessage\": \"An internal server error has ocurred. Please contact support@yourCompany.com.\"}");
        }

        private string GetSanitizedExceptionMessage(Exception exception)
        {
            string message = exception.ToString().Replace("\r\n", ";").Replace("   ", " ");

            return message.Length >= EXCEPTION_MSG_LOG_BYTES_MAX_LENGHT ? message.Substring(0, EXCEPTION_MSG_LOG_BYTES_MAX_LENGHT - 1) + "..." : message;
        }
    }
}