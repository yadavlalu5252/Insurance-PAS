
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;

namespace PolicyIssuance.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class PolicyEndorsementController : ControllerBase
    {
        private readonly IPolicyEndorsementService service;

        public PolicyEndorsementController(
            IPolicyEndorsementService service)
        {
            this.service = service;
        }

        [HttpPost("policies/{id:long}/endorsements")]
        public async Task<IActionResult> Create(
            long id, PolicyEndorsementDTO dto)
        {
            var result = await service.CreateAsync(id, dto);

            if (result == null)
                return BadRequest(
                    "Policy not found, not Active, or endorsement data is invalid.");

            return Ok(result);
        }

        [HttpGet("policies/{id:long}/endorsements")]
        public async Task<IActionResult> GetByPolicyId(long id)
        {
            var result = await service.GetByPolicyIdAsync(id);
            return Ok(result);
        }

        [HttpPut("endorsements/{endorsementId:long}/approve")]
        public async Task<IActionResult> Approve(long endorsementId)
        {
            string approved_by = User.Identity?.Name ?? "system";

            bool result = await service.ApproveAsync(
                endorsementId, approved_by);

            if (!result)
                return BadRequest(
                    "Endorsement not found or is not Pending.");

            return Ok(new { message = "Endorsement approved successfully." });
        }

        [HttpPut("endorsements/{endorsementId:long}/reject")]
        public async Task<IActionResult> Reject(long endorsementId)
        {
            string approved_by = User.Identity?.Name ?? "system";

            bool result = await service.RejectAsync(
                endorsementId, approved_by);

            if (!result)
                return BadRequest(
                    "Endorsement not found or is not Pending.");

            return Ok(new { message = "Endorsement rejected successfully." });
        }
    }
}
