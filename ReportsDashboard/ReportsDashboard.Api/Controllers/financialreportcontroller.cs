using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class financialreportcontroller : ControllerBase
    {
        private readonly Ifinancialreport service;

        public financialreportcontroller(Ifinancialreport service)
        {
            this.service = service;
        }

        [HttpGet("financial")]
        public async Task<IActionResult> getfinancial()
        {
            var data = await service.getfinancial();

            return Ok(data);
        }
    }
}
