using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using ReportsDashboard.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Infrastructure.Services
{
    public class agentreportservice : Iagentreport
    {
        private readonly AgentClient client;

        public agentreportservice(AgentClient client)
        {
            this.client = client;
        }

        public async Task<List<AgentReportDTO>?> getagents()
        {
            return await client.getagents();
        }
    }
}
