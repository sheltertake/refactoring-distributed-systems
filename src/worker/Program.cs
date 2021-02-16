using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http.Json;
using worker.Services;

namespace worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    var options = configuration.GetSection("AppSettings");

                    services.AddHttpClient(nameof(AppService), c => c.BaseAddress = options.GetValue<Uri>("AppUrl"));
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IAppService, AppService>();
                });
    }
}
