using app.Events;
using System.Net.Http;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IBusService
    {
        Task Publish(OrderCreatedEvent orderCreatedEvent);
    }
    public class BusService : IBusService
    {
        private readonly HttpClient Client;
        public BusService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(BusService));
        }

        public async Task Publish(OrderCreatedEvent orderCreatedEvent)
        {
            var response = await Client.PostAsJsonAsync("/", orderCreatedEvent);
            response.EnsureSuccessStatusCode();
        }
    }
}
