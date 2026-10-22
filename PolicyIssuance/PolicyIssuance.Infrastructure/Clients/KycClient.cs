
using System.Net;
using System.Net.Http.Json;
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Infrastructure.Clients
{
    public class KycClient
    {
        private readonly HttpClient client;

        public KycClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<KycServiceDTO?> FindKycByCustomerId(long customerId)
        {
            var response = await client.GetAsync(
                $"api/v1/kyc/customer/{customerId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<ServiceResponseDTO<KycServiceDTO>>();

            return result?.data;
        }
    }
}
