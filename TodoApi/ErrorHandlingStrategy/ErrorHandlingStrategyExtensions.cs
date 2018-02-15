using Microsoft.AspNetCore.Builder;

namespace TodoApi.ErrorHandlingStrategy
{
    public static class ErrorHandlingStrategyExtensions
    {
        public static IApplicationBuilder UseErrorHandlingStrategy(
            this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LoggingMiddleware>();
            builder.UseMiddleware<ExceptionHandlingMiddleware>();

            return builder;
        }
    }
}