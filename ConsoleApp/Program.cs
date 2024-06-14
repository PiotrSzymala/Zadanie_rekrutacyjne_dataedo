using System;
using ConsoleApp.Interfaces;
using ConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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
                try
                {
                    var parser = services.GetRequiredService<Parser>();
                    parser.Do("sampleFile1.csv", "dataSource.csv");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    ConfigureServices(services));

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataImporter, DataImporter>();
            services.AddScoped<IDataSourceLoader, DataSourceLoader>();
            services.AddScoped<IDataMatcher, DataMatcher>();
            services.AddScoped<IDataPrinter, DataPrinter>();

            services.AddScoped<Parser>();
        }
    }
}
