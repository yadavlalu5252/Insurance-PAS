using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PolicyServicing.Infrastructure.Config
{
    public class PremiumClient
    {
        HttpClient client;

        public PremiumClient(HttpClient client)
        {
            this.client = client;
            
        }

        public async Task<List<PremiumScheduleDTO>> GetPremiumSchedule(long policyId)
        {
            return await client.GetFromJsonAsync<List<PremiumScheduleDTO>>($"/api/v1/premium-schedules/{policyId}");
        }

        public async Task<bool> UpdateInstallment(long scheduleId, object updatedInstallment)
        {
            var response = await client.PutAsJsonAsync($"/api/v1/premium-schedules/{scheduleId}", updatedInstallment);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> GeneratePremiumSchedule(long policyId)
        {
            var payload = new { policy_Id = policyId };
            var response = await client.PostAsJsonAsync("/api/v1/premium-schedules", payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateRefund(object refundPayload)
        {
            var response = await client.PostAsJsonAsync("/api/v1/refunds", refundPayload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PaymentHistoryDTO>> GetPaymentHistory(long policyId)
        {
            return await client.GetFromJsonAsync<List<PaymentHistoryDTO>>($"/api/v1/payments/history/{policyId}");
        }
    }
}
