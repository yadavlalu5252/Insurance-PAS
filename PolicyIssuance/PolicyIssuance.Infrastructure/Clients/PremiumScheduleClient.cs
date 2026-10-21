using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interfaces;
using System.Net.Http.Json;

namespace Policy.Infrastructure.Clients
{
    public class PremiumScheduleClient : IPremiumScheduleClient
    {
        private readonly HttpClient client;

        public PremiumScheduleClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<bool> GeneratePremiumSchedule(
            GeneratePremiumScheduleDto request)
        {
            var response = await client.PostAsJsonAsync(
                "api/PremiumSchedule/internal/generate",
                request);

            return response.IsSuccessStatusCode;
        }
    }
}