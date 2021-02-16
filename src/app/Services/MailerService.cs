using app.Entities;
using app.Models;
using Microsoft.Extensions.Logging;
using System;
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
        private readonly ILogger<MailerService> logger;

        public MailerService(IHttpClientFactory httpClientFactory,
                             ILogger<MailerService> logger)
        {
            Client = httpClientFactory.CreateClient(nameof(MailerService));
            this.logger = logger;
        }
        public async Task<MockReportResponse> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<MockReportResponse>("/");
            return ret;
        }
        public async Task SendPaymentSuccessEmailAsync(Order order)
        {
            try
            {
                var response = await Client.PostAsJsonAsync("/", order);
                if (!response.IsSuccessStatusCode)
                    logger.LogWarning($"{response.StatusCode}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}
