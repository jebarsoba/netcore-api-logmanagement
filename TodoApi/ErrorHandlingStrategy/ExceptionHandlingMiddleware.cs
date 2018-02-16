using Microsoft.AspNetCore.Http;
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

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
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
                RequestUri = context.Request.Path,
                RequestMethod = context.Request.Method,
                ExceptionType = exception.GetType().ToString(),
                ExceptionMessage = exception.Message,
                RequestIpAddress = RequestHelper.GetIp(context),
                RequestThreadId = Thread.CurrentThread.ManagedThreadId,
                RequestContentType = context.Request.ContentType,
                RequestContentBody = await RequestHelper.GetBody(context.Request),
                ExceptionFullMessage = this.GetSanitizedExceptionMessage(exception)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorLogEntry));
        }

        private string GetSanitizedExceptionMessage(Exception exception)
        {
            return exception.ToString().Replace("\r\n", ";").Replace("   ", " ");
        }
    }
}