
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class underwritingreportcontroller : ControllerBase
    {
        Iunderwritingreport service;

        public underwritingreportcontroller(Iunderwritingreport service)
        {
            this.service = service;
        }

        [HttpGet("underwriting")]
        public async Task<IActionResult> getunderwriting()
        {
            var data = await service.getunderwriting();
            return Ok(data);
        }
    }
}
