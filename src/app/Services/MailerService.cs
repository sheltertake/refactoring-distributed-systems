using app.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IMailerService
    {
        Task SendPaymentSuccessEmailAsync(Order order);
        Task<IEnumerable<Order>> GetAsync();
    }
    public class MailerService : IMailerService
    {
        private readonly HttpClient Client;
        public MailerService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(MailerService));
        }
        public async Task<IEnumerable<Order>> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<IEnumerable<Order>>("/");
            return ret;
        }
        public async Task SendPaymentSuccessEmailAsync(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
