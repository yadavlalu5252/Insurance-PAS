using ReportsDashboard.Application.DTO;
using System.Net.Http.Json;
namespace ReportsDashboard.Infrastructure.Config
{
    public class CustomerClient
    {
        HttpClient client;
        public CustomerClient(HttpClient client)
        {
            this.client = client;
        }
        public async Task<List<CustomerDTO>?> getcustomers()
        {
            return await client.GetFromJsonAsync<List<CustomerDTO>>("/api/v1/customers");
        }
    }
}
