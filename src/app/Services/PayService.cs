using app.Entities;
using app.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IPayService
    {
        Task PostPaymentAsync(Order order);
        Task<MockReportResponse> GetAsync();
    }

    public class PayService : IPayService
    {
        private readonly HttpClient Client;
        private readonly ILogger<PayService> logger;

        public PayService(IHttpClientFactory httpClientFactory,
                          ILogger<PayService> logger)
        {
            Client = httpClientFactory.CreateClient(nameof(PayService));
            this.logger = logger;
        }

        public async Task<MockReportResponse> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<MockReportResponse>( "/");
            return ret;
        }
        public async Task PostPaymentAsync(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
