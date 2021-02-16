using app.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IBusService
    {
        Task Publish(Order order);
        Task<IEnumerable<Order>> GetAsync();
    }
    public class BusService : IBusService
    {
        private readonly HttpClient Client;
        public BusService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(BusService));
        }
        public async Task<IEnumerable<Order>> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<IEnumerable<Order>>("/");
            return ret;
        }
        public async Task Publish(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
