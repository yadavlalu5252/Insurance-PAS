
using System.Net;
using System.Text.Json;
using QuoteGeneration.Application.DTO;

namespace QuoteGeneration.Infrastructure.Config
{
    public class InsuranceProductClient
    {
        private readonly HttpClient client;

        public InsuranceProductClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<InsuranceProductDTO?> GetProductByIdAsync(int id)
        {
            var response = await client.GetAsync(
                $"api/v1/insurance-products/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            using var document = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            if (!document.RootElement.TryGetProperty(
                    "data", out var data))
            {
                return null;
            }

            return JsonSerializer.Deserialize<InsuranceProductDTO>(
                data.GetRawText(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<RiderMasterDTO?> GetRiderByIdAsync(int id)
        {
            var response = await client.GetAsync(
                $"api/v1/riders/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            using var document = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync());

            if (!document.RootElement.TryGetProperty("data", out var data))
                return null;

            return JsonSerializer.Deserialize<RiderMasterDTO>(
                data.GetRawText(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

    }
}
