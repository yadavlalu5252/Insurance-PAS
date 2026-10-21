using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ClaimManagement.Infrastructure.Config
{
    public class PolicyClient
    {
        HttpClient client;

        public PolicyClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<PolicyInfoDTO?> FindPolicyById(long id)
        {
            try
            {
                return await client.GetFromJsonAsync<PolicyInfoDTO>($"/api/v1/policies/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;   // policy does not exist
            }
        }
    }
}
