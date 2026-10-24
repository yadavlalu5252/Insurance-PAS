
using ReportsDashboard.Application.DTO;
using System.Net.Http.Json;

namespace ReportsDashboard.Infrastructure.Config
{
    public class PremiumClient
    {
        HttpClient client;

        public PremiumClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<PremiumDTO>?> getpremiums()
        {
            return await client.GetFromJsonAsync<List<PremiumDTO>>("/api/v1/premiums");
        }
    }
}
