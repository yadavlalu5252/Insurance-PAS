
using ReportsDashboard.Application.DTO;
using System.Net.Http.Json;

namespace ReportsDashboard.Infrastructure.Config
{
    public class PolicyClient
    {
        HttpClient client;

        public PolicyClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<PolicyDTO>?> getpolicies()
        {
            return await client.GetFromJsonAsync<List<PolicyDTO>>("/api/v1/policies");
        }
    }
}
