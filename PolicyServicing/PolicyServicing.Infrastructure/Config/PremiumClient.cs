using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Config
{

    public class PremiumApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class PremiumClient
    {
        HttpClient client;

        public PremiumClient(HttpClient client)
        {
            this.client = client;
        }

        private void SetAuthHeader(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }
        }

        public async Task<List<PremiumScheduleDTO>> GetPremiumSchedule(long policyId, string token)
        {
            SetAuthHeader(token);
  
            var response = await client.GetFromJsonAsync<PremiumApiResponse<List<PremiumScheduleDTO>>>($"/api/v1/premium-schedules/{policyId}");
            return response?.Data ?? new List<PremiumScheduleDTO>();
        }

        public async Task<bool> UpdateInstallment(long scheduleId, object updatedInstallment, string token)
        {
            SetAuthHeader(token);
            var response = await client.PutAsJsonAsync($"/api/v1/premium-schedules/{scheduleId}", updatedInstallment);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> GeneratePremiumSchedule(object payload, string token)
        {
            SetAuthHeader(token);


            var response = await client.PostAsJsonAsync("/api/v1/premium-schedules/internal/generate", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateRefund(object refundPayload, string token)
        {
            SetAuthHeader(token);
            var response = await client.PostAsJsonAsync("/api/v1/refunds", refundPayload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PaymentHistoryDTO>> GetPaymentHistory(long policyId, string token)
        {
            SetAuthHeader(token);

            return await client.GetFromJsonAsync<List<PaymentHistoryDTO>>($"/api/v1/payments/history/{policyId}");
        }
    }
}