using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System;
using System.Threading.Tasks;

namespace PolicyServicing.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class PolicyRenewalController : ControllerBase
    {
        private readonly IPolicyRenew _renewalService;

        public PolicyRenewalController(IPolicyRenew renewalService)
        {
            _renewalService = renewalService;
        }

        // ==========================================
        // POST /api/v1/policies/{id}/renew
        // ==========================================
        [HttpPost("policies/{id}/renew")]
        public async Task<IActionResult> RenewPolicy(long id, [FromQuery] long customerId, [FromBody] RenewalRequestDTO request)
        {
            if (request == null)
                return BadRequest("Invalid renewal payload.");

            if (customerId <= 0)
                return BadRequest("Valid Customer ID is required.");

            try
            {
                // Call the service to execute the renewal
                var result = await _renewalService.ProcessPolicyRenewalAsync(customerId, id, request);

                return Ok(new
                {
                    Message = result
                });
            }
            catch (Exception ex)
            {
                // Catches business rules like "Policy not found", "Pending premiums", etc.
                return BadRequest(new { Error = ex.Message });
            }
        }

        // ==========================================
        // GET /api/v1/renewals/{id}
        // ==========================================
        [HttpGet("renewals/{id}")]
        public async Task<IActionResult> GetRenewalDetails(long id)
        {
            var result = await _renewalService.GetRenewalDetailsAsync(id);

            if (result == null)
                return NotFound(new { Message = "Renewal record not found." });

            return Ok(result);
        }
    }
}