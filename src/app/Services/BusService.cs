using app.Entities;
using app.Models;
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
        public BusService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(BusService));
        }
        public async Task<MockReportResponse> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<MockReportResponse>("/");
            return ret;
        }
        public async Task Publish(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
