using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.SumoLogic;
using TodoApi.ErrorHandlingStrategy;
using TodoApi.Models;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var isEventFromLoggingMiddleware = Matching.FromSource<LoggingMiddleware>();
            var isEventFromExceptionHandlingMiddleware = Matching.FromSource<ExceptionHandlingMiddleware>();

            Log.Logger = new LoggerConfiguration()
                            .WriteTo.File("C:\\TodoApiLogs.txt")
                            /*.WriteTo.SumoLogic(
                                endpointUrl: "[YOUR SUMO COLLECTOR URL]",
                                outputTemplate: "{Message}"
                            )*/
                            .Filter.ByIncludingOnly(logEvent => isEventFromLoggingMiddleware(logEvent) || isEventFromExceptionHandlingMiddleware(logEvent))
                            .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("demoDatabase"));
            services.AddScoped<ITodoRepository, TodoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseErrorHandlingStrategy();

            loggerFactory
                .AddSerilog();

            app.UseMvc();
        }
    }
}