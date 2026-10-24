
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class agentreportcontroller : Controller
    {
        Iagentreport service;

        public agentreportcontroller(Iagentreport service)
        {
            this.service = service;
        }

        [HttpGet("agents")]
        public async Task<IActionResult> getagents()
        {
            var data = await service.getagents();
            return Ok(data);
        }
    }
}
