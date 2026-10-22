
using System.Net;
using System.Net.Http.Json;
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Infrastructure.Clients
{
    public class QuoteClient
    {
        private readonly HttpClient client;

        public QuoteClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<QuoteServiceDTO?> FindQuoteById(long id)
        {
            var response = await client.GetAsync($"api/Quote/Get/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<ServiceResponseDTO<QuoteServiceDTO>>();

            return result?.data;
        }
    }
}
