using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks; 

namespace PolicyServicing.Infrastructure.Config
{
    public class PolicyClient
    {
        HttpClient client;

        public PolicyClient(HttpClient client)
        {
            this.client = client;
        }

        private void SetAuthHeader(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }
        }

        public async Task<bool> CreatePolicyEndorsement(long policyId, object endorsementData, string token)
        {
            SetAuthHeader(token);
            var response = await client.PostAsJsonAsync($"/api/v1/policies/{policyId}/endorsements", endorsementData);
            return response.IsSuccessStatusCode;
        }

        public async Task<PolicyDTO> GetPolicyById(long policyId, string token)
        {
            SetAuthHeader(token);
         
            return await client.GetFromJsonAsync<PolicyDTO>($"/api/v1/policies/{policyId}");
        }

        public async Task<bool> UpdatePolicy(long policyId, object updatePayload, string token)
        {
            SetAuthHeader(token);
            var response = await client.PutAsJsonAsync($"/api/v1/policies/{policyId}", updatePayload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelPolicy(long policyId, string token)
        {
            SetAuthHeader(token);
            var response = await client.PutAsync($"/api/v1/policies/{policyId}/cancel", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PolicyCoverageDTO>> GetPolicyCoverages(long policyId, string token)
        {
            SetAuthHeader(token);

            return await client.GetFromJsonAsync<List<PolicyCoverageDTO>>($"/api/v1/policies/{policyId}/coverages");
        }

        // Inside PolicyClient.cs
        public async Task<bool> UpdatePolicyCoverage(long policyId, long coverageId, object coveragePayload, string token)
        {
            SetAuthHeader(token);
            // Hits her new endpoint
            var response = await client.PutAsJsonAsync($"/api/v1/policies/{policyId}/coverages/{coverageId}", coveragePayload);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdatePolicyRiders(long policyId, object riderPayload, string token)
        {
            SetAuthHeader(token);
           
            var response = await client.PutAsJsonAsync($"/api/v1/policies/{policyId}/riders", riderPayload);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PolicyRiderDTO>> GetPolicyRiders(long policyId, string token)
        {
            SetAuthHeader(token);
            return await client.GetFromJsonAsync<List<PolicyRiderDTO>>($"/api/v1/policies/{policyId}/riders");
        }


        public async Task<bool> RenewPolicy(long policyId, DateTime newExpiryDate, string token)
        {
            SetAuthHeader(token);
            var response = await client.PutAsJsonAsync($"/api/v1/policies/{policyId}/renew", new { NewExpiryDate = newExpiryDate });
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PolicyDTO>> GetPoliciesByCustomerId(long customerId, string token)
        {
            SetAuthHeader(token);
            // Hits her new endpoint: /api/v1/policies/customer/{customerId}
            return await client.GetFromJsonAsync<List<PolicyDTO>>($"/api/v1/policies/customer/{customerId}");
        }
    }
}