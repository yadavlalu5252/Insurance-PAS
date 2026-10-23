
using Microsoft.AspNetCore.Mvc;
using ReportsDashboard.Application.DTO;
using ReportsDashboard.Application.Interfaces;

namespace ReportsDashboard.Api.Controllers
{
    [Route("api/v1/reports/schedule")]
    [ApiController]
    public class scheduledreportcontroller : ControllerBase
    {
        private readonly Ischeduledreport service;

        public scheduledreportcontroller(Ischeduledreport service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> addschedule(savescheduledreportdto dto)
        {
            var data = await service.addschedule(dto);

            if (data == null)
            {
                return BadRequest("Report ID not found");
            }

            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> getschedules()
        {
            var data = await service.getschedules();

            return Ok(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateschedule(long id, savescheduledreportdto dto)
        {
            var result = await service.updateschedule(id, dto);

            if (!result)
            {
                return NotFound("Schedule or report ID not found");
            }

            return Ok("Schedule updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteschedule(long id)
        {
            var result = await service.deleteschedule(id);

            if (!result)
            {
                return NotFound("Schedule not found");
            }

            return Ok("Schedule deleted successfully");
        }
    }
}
