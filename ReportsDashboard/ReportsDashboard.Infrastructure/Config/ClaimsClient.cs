
using ReportsDashboard.Application.DTO;
using System.Net.Http.Json;

namespace ReportsDashboard.Infrastructure.Config
{
    public class ClaimsClient
    {
        HttpClient client;

        public ClaimsClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<ClaimsDTO>?> getclaims()
        {
            return await client.GetFromJsonAsync<List<ClaimsDTO>>("/api/v1/claims");
        }
    }
}
