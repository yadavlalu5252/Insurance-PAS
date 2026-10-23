
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class PolicyReportController : ControllerBase
    {
        IPolicyReport service;

        public PolicyReportController(IPolicyReport service)
        {
            this.service = service;
        }

        [HttpGet("policies")]
        public async Task<IActionResult> getpolicies()
        {
            var data = await service.getpolicies();
            return Ok(data);
        }
    }
}
