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
    public class Program
    {
        private static ILogger<Program> _logger;
        private static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            _logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var parser = services.GetRequiredService<Parser>();
                    parser.Do("sampleFile1.csv", "dataSource.csv");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error occurred while setting up the application environment.");

            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Creates and configures a host builder with the specified command-line arguments.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>An <see cref="IHostBuilder"/> configured with the application's services and logging.</returns>
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


        /// <summary>
        /// Configures the application's services.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IConsoleWriter, ConsoleWriter>();
            services.AddScoped<IFileReader, FileReader>();

            services.AddScoped<IDataImporter, DataImporter>();
            services.AddScoped<IDataSourceLoader, DataSourceLoader>();
            services.AddScoped<IDataMatcher, DataMatcher>();
            services.AddScoped<IDataPrinter, DataPrinter>();

            services.AddLogging();
            services.AddScoped<Parser>();
        }
    }
}
