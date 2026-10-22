
using System.Net;
using System.Net.Http.Json;
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Infrastructure.Clients
{
    public class PremiumPaymentClient
    {
        private readonly HttpClient client;

        public PremiumPaymentClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<bool> CheckPaymentHistory(long policyId)
        {
            var response = await client.GetAsync(
                $"api/v1/payments/history/{policyId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();

            return true;
        }
    }
}
