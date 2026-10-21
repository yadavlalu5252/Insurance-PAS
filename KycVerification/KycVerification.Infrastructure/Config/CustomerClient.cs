using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Infrastructure.Config
{
    public class CustomerClient
    {
        private readonly HttpClient _client;

        public CustomerClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<CustomerDTO?> FindCustomerById(int id)
        {
            return await _client.GetFromJsonAsync<CustomerDTO>($"/api/v1/customers/{id}");
        }
    }
}
