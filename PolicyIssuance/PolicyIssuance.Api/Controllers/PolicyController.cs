
using Microsoft.AspNetCore.Mvc;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;

namespace PolicyIssuance.Api.Controllers
{
    [ApiController]
    [Route("api/v1/policies")]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService service;

        public PolicyController(IPolicyService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PolicyDTO dto)
        {
            var result = await service.CreateAsync(dto);

            if (result == null)
                return BadRequest("Invalid policy data or quote already used.");

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.policy_id },
                result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAllAsync());
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await service.GetByIdAsync(id);

            if (result == null)
                return NotFound("Policy not found.");

            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, PolicyDTO dto)
        {
            var result = await service.UpdateAsync(id, dto);

            if (result == null)
                return BadRequest("Policy not found, invalid data, or policy is not Draft.");

            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            bool result = await service.DeleteAsync(id);

            if (!result)
                return NotFound("Policy not found or cannot be cancelled.");

            return Ok(new { message = "Policy cancelled successfully." });
        }

        [HttpPut("{id:long}/activate")]
        public async Task<IActionResult> Activate(long id)
        {
            string performed_by = User.Identity?.Name ?? "system";
            bool result = await service.ActivateAsync(id, performed_by);

            if (!result)
                return BadRequest(
                    "Policy cannot be activated. Verify underwriting approval and first premium payment.");

            return Ok(new { message = "Policy activated successfully." });
        }

        [HttpPut("{id:long}/suspend")]
        public async Task<IActionResult> Suspend(long id)
        {
            string performed_by = User.Identity?.Name ?? "system";
            bool result = await service.SuspendAsync(id, performed_by);

            if (!result)
                return BadRequest("Policy not found or is not Active.");

            return Ok(new { message = "Policy suspended successfully." });
        }

        [HttpPut("{id:long}/cancel")]
        public async Task<IActionResult> Cancel(long id)
        {
            string performed_by = User.Identity?.Name ?? "system";
            bool result = await service.CancelAsync(id, performed_by);

            if (!result)
                return BadRequest("Policy not found or cannot be cancelled.");

            return Ok(new { message = "Policy cancelled successfully." });
        }

        [HttpPut("{id:long}/expire")]
        public async Task<IActionResult> Expire(long id)
        {
            string performed_by = User.Identity?.Name ?? "system";
            bool result = await service.ExpireAsync(id, performed_by);

            if (!result)
                return BadRequest(
                    "Policy not found, not Active, or expiry date has not passed.");

            return Ok(new { message = "Policy expired successfully." });
        }
    }
}
