using Microsoft.AspNetCore.Builder;

namespace TodoApi.ErrorHandlingStrategy
{
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingStrategy(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
