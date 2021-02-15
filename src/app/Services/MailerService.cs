using app.Entities;
using System.Net.Http;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IMailerService
    {
        Task SendPaymentSuccessEmailAsync(Order order);
    }
    public class MailerService : IMailerService
    {
        private readonly HttpClient Client;
        public MailerService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(MailerService));
        }

        public async Task SendPaymentSuccessEmailAsync(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
