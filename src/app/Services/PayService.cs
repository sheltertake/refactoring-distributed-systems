﻿using app.Entities;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IPayService
    {
        Task PostPaymentAsync(Order order);
        Task<IEnumerable<Order>> GetAsync();
    }

    public class PayService : IPayService
    {
        private readonly HttpClient Client;
        public PayService(IHttpClientFactory httpClientFactory)
        {
            Client = httpClientFactory.CreateClient(nameof(PayService));
        }

        public async Task<IEnumerable<Order>> GetAsync()
        {
            var ret = await Client.GetFromJsonAsync<IEnumerable<Order>>("/");
            return ret;
        }

        public async Task PostPaymentAsync(Order order)
        {
            var response = await Client.PostAsJsonAsync("/", order);
            response.EnsureSuccessStatusCode();
        }
    }
}
