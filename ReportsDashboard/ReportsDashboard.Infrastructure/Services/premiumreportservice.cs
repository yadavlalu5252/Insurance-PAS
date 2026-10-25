using ReportsDashboard.Infrastructure.Config;
using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Infrastructure.Services
{
    public class premiumreportservice : Ipremiumreport
    {
        PremiumClient client;

        public premiumreportservice(PremiumClient client)
        {
            this.client = client;
        }

        public async Task<List<PremiumDTO>?> getpremiums()
        {
            return await client.getpremiums();
        }
    }
}
