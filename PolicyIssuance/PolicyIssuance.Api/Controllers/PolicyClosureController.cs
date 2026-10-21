

using Microsoft.AspNetCore.Mvc;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;

namespace PolicyIssuance.Api.Controllers
{
    [Route("policyclosures")]
    [ApiController]
    public class PolicyClosureController : ControllerBase
    {
        IPolicyClosureService service;

        public PolicyClosureController(IPolicyClosureService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddClosure(PolicyClosureDTO dto)
        {
            try
            {
                await service.AddClosure(dto);
                return Ok(new { msg = "Closure request submitted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> FindClosureById(long id)
        {
            var data = await service.FindClosureById(id);

            if (data == null)
                return NotFound(new { msg = "Closure request not found" });

            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> FetchAllClosure()
        {
            var data = await service.FetchAllClosure();
            return Ok(data);
        }

        [HttpGet]
        [Route("pending")]
        public async Task<IActionResult> FetchPendingClosure()
        {
            var data = await service.FetchPendingClosure();
            return Ok(data);
        }

        [HttpPut]
        [Route("{id:long}/approve")]
        public async Task<IActionResult> ApproveClosure(
            long id,
            [FromQuery] string approvedBy,
            [FromQuery] decimal? refundAmount)
        {
            var result = await service.ApproveClosure(
                id, approvedBy, refundAmount);

            if (!result)
                return BadRequest(new
                {
                    msg = "Closure request could not be approved"
                });

            return Ok(new { msg = "Closure completed successfully" });
        }

        [HttpPut]
        [Route("{id:long}/reject")]
        public async Task<IActionResult> RejectClosure(
            long id,
            [FromQuery] string rejectionReason,
            [FromQuery] string rejectedBy)
        {
            var result = await service.RejectClosure(
                id, rejectionReason, rejectedBy);

            if (!result)
                return BadRequest(new
                {
                    msg = "Closure request could not be rejected"
                });

            return Ok(new { msg = "Closure request rejected" });
        }
    }
}
