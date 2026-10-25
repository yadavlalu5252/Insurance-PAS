using ReportsDashboard.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace ReportsDashboard.Infrastructure.Config
{
    public class AgentClient
    {
        HttpClient client;

        public AgentClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<AgentReportDTO>?> getagents()
        {
            return await client.GetFromJsonAsync<List<AgentReportDTO>>(
                "/api/agents");
        }
    }
}
