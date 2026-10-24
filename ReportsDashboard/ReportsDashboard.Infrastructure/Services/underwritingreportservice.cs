using ReportsDashboard.Infrastructure.Config;
using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Infrastructure.Services
{
    public class underwritingreportservice : Iunderwritingreport
    {
        UnderwritingClient client;

        public underwritingreportservice(UnderwritingClient client)
        {
            this.client = client;
        }

        public async Task<List<UnderwritingDTO>?> getunderwriting()
        {
            return await client.getunderwriting();
        }
    }
}
