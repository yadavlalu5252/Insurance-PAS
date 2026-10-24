
using ReportsDashboard.Infrastructure.Config;
using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Infrastructure.Services
{
    public class policyreportservice : IPolicyReport
    {
        private readonly PolicyClient client;

        public policyreportservice(PolicyClient client)
        {
            this.client = client;
        }

        public async Task<List<PolicyDTO>?> getpolicies()
        {
            return await client.getpolicies();
        }
    }
}
