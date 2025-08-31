using Serilog;
using Serilog.Events;

namespace ProductDemo.Logging
{
    public static class LogConfigurator
    {
        public static void ConfigureLogging(IConfiguration configuration, string environment)
        {
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Console/File + static enrichers from appsettings
                .Enrich.WithCorrelationId()            // dynamic: request-level correlation
                .Enrich.WithProperty("Environment", environment); // dynamic: always attach env name

            // Only add Application Insights sink in Staging/Prod
            if (environment == "Staging" || environment == "Production")
            {
                var aiConnectionString = configuration["ApplicationInsights:ConnectionString"];
                if (!string.IsNullOrEmpty(aiConnectionString))
                {
                    loggerConfig = loggerConfig.WriteTo.ApplicationInsights(
                        aiConnectionString,
                        TelemetryConverter.Traces,
                        restrictedToMinimumLevel: LogEventLevel.Information
                    );
                }
            }

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}