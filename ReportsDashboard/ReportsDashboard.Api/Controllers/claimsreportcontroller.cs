using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class claimsreportcontroller : Controller
    {
        Iclaimsreport service;

        public claimsreportcontroller(Iclaimsreport service)
        {
            this.service = service;
        }

        [HttpGet("claims")]
        public async Task<IActionResult> getclaims()
        {
            var data = await service.getclaims();
            return Ok(data);
        }
    }
}