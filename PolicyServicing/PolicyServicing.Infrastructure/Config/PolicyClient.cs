using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PolicyServicing.Infrastructure.Config
{
    public class PolicyClient
    {
        HttpClient client;

        public PolicyClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<bool> CreatePolicyEndorsement(long policyId, object endorsementData)
        {
            var response = await client.PostAsJsonAsync($"/api/v1/policies/{policyId}/endorsements", endorsementData);
            return response.IsSuccessStatusCode;
        }

        public async Task<PolicyDetailsDTO> GetPolicyById(long policyId)
        {
            return await client.GetFromJsonAsync<PolicyDetailsDTO>($"/api/v1/policies/{policyId}");
        }

        public async Task<bool> UpdatePolicy(long policyId, object updatePayload)
        {
            var response = await client.PutAsJsonAsync($"/api/v1/policies/{policyId}", updatePayload);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> CancelPolicy(long policyId)
        {
            var response = await client.PutAsync($"/api/v1/policies/{policyId}/cancel",null);

            return response.IsSuccessStatusCode;

        }


    }
}
