using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PolicyServicing.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class PolicyRenewalController : ControllerBase
    {
        private readonly IPolicyRenew _renewalService;

        public PolicyRenewalController(IPolicyRenew renewalService)
        {
            _renewalService = renewalService;
        }

        [HttpPost("policies/{id}/renew")]
        public async Task<IActionResult> RenewPolicy(long id,  [FromBody] RenewalRequestDTO request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out long customerId))
                return Unauthorized("Customer ID missing from token.");

            try
            {
                
                var result = await _renewalService.ProcessPolicyRenewalAsync(customerId, id, request);

                return Ok(new
                {
                    Message = result
                });
            }
            catch (Exception ex)
            {
              
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpGet("renewals/{id}")]
        public async Task<IActionResult> GetRenewalDetails(long id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out long customerId))
                return Unauthorized("Customer ID missing.");

            var result = await _renewalService.GetRenewalDetailsAsync(id, customerId);

            if (result == null)
                return NotFound(new { Message = "Renewal record not found or unauthorized." });

            return Ok(result);
        }
    }
}