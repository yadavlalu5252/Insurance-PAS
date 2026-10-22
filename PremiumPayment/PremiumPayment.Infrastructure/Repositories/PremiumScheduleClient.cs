using PremiumPayment.Application.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PremiumPayment.Infrastructure.Repositories
{
    public class PremiumScheduleClient:IPremiumScheduleClient
    {
        private readonly HttpClient httpClient;

        public PremiumScheduleClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task UpdatePaymentStatusAsync(
            int scheduleId,
            decimal amountPaid,
            int paymentId)
        {
            var request = new
            {
                schedule_id = scheduleId,
                amount_paid = amountPaid,
                payment_id = paymentId
            };

            var response = await httpClient.PutAsJsonAsync(
                $"api/v1/premium-schedules/{scheduleId}/payment",
                request);

            response.EnsureSuccessStatusCode();
        }

    }
}
