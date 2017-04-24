using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace TodoApi.ErrorHandlingStrategy
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            //TODO: Log request

            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }
    }
}
