using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace worker.Services
{
    public interface IAppService 
    {
        Task PublishAsync();
    }
    public class AppService : IAppService
    {
        private readonly HttpClient Client;
        private readonly ILogger<AppService> logger;

        public AppService(IHttpClientFactory httpClientFactory,
                          ILogger<AppService> logger)
        {
            Client = httpClientFactory.CreateClient(nameof(AppService));
            this.logger = logger;
        }
        
        public async Task PublishAsync()
        {
            var response = await Client.PostAsync("/bus", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
