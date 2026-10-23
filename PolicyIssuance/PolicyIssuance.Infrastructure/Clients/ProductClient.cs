
using System.Net;
using System.Net.Http.Json;
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Infrastructure.Clients
{
    public class ProductClient
    {
        private readonly HttpClient client;

        public ProductClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<ProductServiceDTO?> FindProductById(long id)
        {
            var response = await client.GetAsync($"api/v1/products/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<ServiceResponseDTO<ProductServiceDTO>>();

            return result?.data;
        }
    }
}
