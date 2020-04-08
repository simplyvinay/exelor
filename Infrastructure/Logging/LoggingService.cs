using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Exelor.Infrastructure.Logging
{
    public static class LoggingService
    {
        public static void AddSerilogLogging(
            this ILoggerFactory loggerFactory)
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                //.Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            loggerFactory.AddSerilog(log);
            Log.Logger = log;
        }
    }
}