
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace QuoteGeneration.Infrastructure.Config
{
    public class UnderwritingClient
    {
        private readonly HttpClient client;

        public UnderwritingClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<UnderwritingRiskDTO?> GetRiskByQuoteIdAsync(int quoteId)
        {
            var response = await client.GetAsync($"/api/v1/underwriting/quote/{quoteId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!result.TryGetProperty("data", out var data))
                return null;

            int underwritingId = data.GetProperty("underwriting_id").GetInt32();

            var riskResponse = await client.PostAsync($"/api/RiskScore/Calculate/{underwritingId}", null);

            if (riskResponse.StatusCode == HttpStatusCode.NotFound)
                return null;

            riskResponse.EnsureSuccessStatusCode();

            return await riskResponse.Content.ReadFromJsonAsync<UnderwritingRiskDTO>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }

    public class UnderwritingRiskDTO
    {
        public int underwriting_id { get; set; }
        public int total_score { get; set; }
        public string risk_category { get; set; } = string.Empty;
    }
}
