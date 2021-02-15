using app.Entities;
using System.Net.Http;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IPayService
    {
        Task PostPaymentAsync(Order order);
    }

    public class PayService : IPayService
    {
        private readonly HttpClient Client;
        public PayService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(PayService));
        }
        public async Task PostPaymentAsync(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
