
using System.Net;
using System.Net.Http.Json;
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Infrastructure.Clients
{
    public class UnderwritingClient
    {
        private readonly HttpClient client;

        public UnderwritingClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<UnderwritingServiceDTO?> FindUnderwritingById(long id)
        {
            var response = await client.GetAsync(
                $"api/v1/underwriting/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<ServiceResponseDTO<UnderwritingServiceDTO>>();

            return result?.data;
        }
    }
}
