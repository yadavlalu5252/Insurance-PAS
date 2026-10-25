using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using ReportsDashboard.Infrastructure.Config;


namespace ReportsDashboard.Infrastructure.Services
{
    public class claimsreportservice : Iclaimsreport
    {
        ClaimsClient client;

        public claimsreportservice(ClaimsClient client)
        {
            this.client = client;
        }

        public async Task<List<ClaimsDTO>?> getclaims()
        {
            return await client.getclaims();
        }
    }
}
