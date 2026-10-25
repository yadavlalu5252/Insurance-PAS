
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class premiumreportcontroller : ControllerBase
    {
        Ipremiumreport service;

        public premiumreportcontroller(Ipremiumreport service)
        {
            this.service = service;
        }

        [HttpGet("premiums")]
        public async Task<IActionResult> getpremiums()
        {
            var data = await service.getpremiums();
            return Ok(data);
        }
    }
}
