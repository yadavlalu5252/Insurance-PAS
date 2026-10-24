using System.Net.Http.Json;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class PolicyIssuanceClient : IPolicyIssuanceClient
    {
        private readonly HttpClient httpClient;

        public PolicyIssuanceClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PolicyDto?> GetPolicyAsync(int policyId)
        {
            if (policyId <= 0)
            {
                return null;
            }

            var response = await httpClient.GetAsync(
                $"api/v1/policies/{policyId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<PolicyDto>();

            return result;
        }

        public Task<string?> GetPremiumFrequency(int policyId)
        {
            throw new NotImplementedException(
                "GetPremiumFrequency has not been implemented yet.");
        }
    }
}
