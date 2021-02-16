using app.Entities;
using app.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IMailerService
    {
        Task SendPaymentSuccessEmailAsync(Order order);
        Task<MockReportResponse> GetAsync();
    }
    public class MailerService : IMailerService
    {
        private readonly HttpClient Client;
        public MailerService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(MailerService));
        }
        public async Task<MockReportResponse> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<MockReportResponse>("/");
            return ret;
        }
        public async Task SendPaymentSuccessEmailAsync(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
