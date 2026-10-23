
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class CustomerReportController : ControllerBase
    {
        ICustomerReport service;

        public CustomerReportController(ICustomerReport service)
        {
            this.service = service;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> getcustomers()
        {
            var data = await service.getcustomers();
            return Ok(data);
        }
    }
}
