using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;
using ReportsDashboard.Infrastructure.Config;

namespace ReportsDashboard.Api.Services
{
    public class customerreportservice : ICustomerReport
    {
        private readonly CustomerClient client;

        public customerreportservice(CustomerClient client)
        {
            this.client = client;
        }

        public async Task<List<CustomerDTO>?> getcustomers()
        {
            
            return await client.getcustomers();
        }
    }
}