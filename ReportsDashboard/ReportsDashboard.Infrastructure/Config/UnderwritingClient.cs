
using ReportsDashboard.Application.DTO;
using System.Net.Http.Json;

namespace ReportsDashboard.Infrastructure.Config
{
    public class UnderwritingClient
    {
        HttpClient client;

        public UnderwritingClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<UnderwritingDTO>?> getunderwriting()
        {
            return await client.GetFromJsonAsync<List<UnderwritingDTO>>("/api/v1/underwriting");
        }
    }
}
