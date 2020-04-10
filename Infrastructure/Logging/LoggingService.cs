using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Exelor.Infrastructure.Logging
{
    public static class LoggingService
    {
        public static void AddSerilogLogging(
            this ILoggerFactory loggerFactory,
            IWebHostEnvironment env)
        {
            /*var log1 = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                //.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
                //.Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .WriteTo.File(new CompactJsonFormatter(), "Logs/logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();*/

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(
                    "appsettings.json",
                    false,
                    true)
                .AddJsonFile(
                    $"appsettings.{env.EnvironmentName}.json",
                    true)
                .AddEnvironmentVariables()
                .Build();

            var log = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            loggerFactory.AddSerilog(log);
            Log.Logger = log;
        }
    }
}