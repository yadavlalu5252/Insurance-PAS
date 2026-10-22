
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Underwriting.Application.DTO;

namespace Underwriting.Infrastructure.Config
{
    public class CustomerRegistrationClient
    {
        private readonly HttpClient client;

        public CustomerRegistrationClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(long id)
        {
            var response = await client.GetAsync($"/api/customers/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<
                JsonElement>();

            if (!result.TryGetProperty("data", out var data))
            {
                return null;
            }

            return JsonSerializer.Deserialize<CustomerResponseDto>(
                data.GetRawText(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
