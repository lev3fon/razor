using System;
using System.Linq;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BadNews
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializeDataBase();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(".logs/start-host-log-.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Creating web host builder");
                var hostBuilder = CreateHostBuilder(args);
                Log.Information("Building web host");
                var host = hostBuilder.Build();
                Log.Information("Running web host");
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseEnvironment(Environments.Development);
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
        }

        private static void InitializeDataBase()
        {
            const int newsArticleCount = 100;

            var generator = new NewsGenerator();
            var articles = generator.GenerateNewsArticles()
                .Take(newsArticleCount)
                .OrderBy(it => it.Id)
                .ToList();

            var repository = new NewsRepository();
            repository.InitializeDataBase(articles);
        }
    }
}
