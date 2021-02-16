using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using worker.Services;

namespace worker
{
    public class Worker : BackgroundService
    {
        private readonly IAppService appService;
        private readonly ILogger<Worker> _logger;

        public Worker(IAppService appService, 
                      ILogger<Worker> logger)
        {
            this.appService = appService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                await appService.PublishAsync();
                
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
