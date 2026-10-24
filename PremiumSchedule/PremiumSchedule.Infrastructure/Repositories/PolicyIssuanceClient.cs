using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class PolicyIssuanceClient : IPolicyIssuanceClient
    {
        private readonly HttpClient httpClient;

        public PolicyIssuanceClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string?> GetPremiumFrequency(int policyId)
        {
            var response = await httpClient.GetAsync(
                $"api/policies/{policyId}/premium-frequency");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result =
                await response.Content.ReadFromJsonAsync<PremiumFrequencyResponse>();

            return result?.PremiumFrequency;
        }

        
    }
}
