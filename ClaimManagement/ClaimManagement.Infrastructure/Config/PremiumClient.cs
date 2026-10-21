using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ClaimManagement.Infrastructure.Config
{
    public class PremiumClient
    {
        HttpClient client;

        public PremiumClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<PremiumInstallmentDTO>?> FindScheduleByPolicy(long policyId)
        {
            try
            {
                return await client.GetFromJsonAsync<List<PremiumInstallmentDTO>>($"/api/v1/premium-schedules/{policyId}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;   // no schedule for this policy
            }
        }
    }
}
