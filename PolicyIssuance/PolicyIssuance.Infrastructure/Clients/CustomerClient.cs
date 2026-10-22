
using System.Net;
using System.Net.Http.Json;
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Infrastructure.Clients
{
    public class CustomerClient
    {
        private readonly HttpClient client;

        public CustomerClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<CustomerServiceDTO?> FindCustomerById(long id)
        {
            var response = await client.GetAsync($"api/customers/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<ServiceResponseDTO<CustomerServiceDTO>>();

            return result?.data;
        }
    }
}
