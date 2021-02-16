using app.Entities;
using app.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IBusService
    {
        Task Publish(Order order);
        Task<MockReportResponse> GetAsync();
    }
    public class BusService : IBusService
    {
        private readonly HttpClient Client;
        private readonly ILogger<BusService> logger;

        public BusService(IHttpClientFactory httpClientFactory, 
                          ILogger<BusService> logger)
        {
            Client = httpClientFactory.CreateClient(nameof(BusService));
            this.logger = logger;
        }
        public async Task<MockReportResponse> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<MockReportResponse>("/");
            return ret;
        }
        public async Task Publish(Order order)
        {
            try
            {
                var response = await Client.PostAsJsonAsync("/", order);
                if(!response.IsSuccessStatusCode)
                    logger.LogWarning($"{response.StatusCode}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
            
        }
    }
}
