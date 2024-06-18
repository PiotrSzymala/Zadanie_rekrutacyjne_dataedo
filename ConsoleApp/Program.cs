using System;
using ConsoleApp.Interfaces;
using ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;


namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    var parser = services.GetRequiredService<Parser>();

                    parser.Do("sampleFile1.csv", "dataSource.csv");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"There was an error processing the data: ");
                }
                finally
                {
                    NLog.LogManager.Shutdown();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    ConfigureServices(services))
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    logging.AddNLog();
                });


        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataImporter, DataImporter>();
            services.AddScoped<IDataSourceLoader, DataSourceLoader>();
            services.AddScoped<IDataMatcher, DataMatcher>();
            services.AddScoped<IDataPrinter, DataPrinter>();

            services.AddLogging();
            services.AddScoped<Parser>();
        }
    }
}
