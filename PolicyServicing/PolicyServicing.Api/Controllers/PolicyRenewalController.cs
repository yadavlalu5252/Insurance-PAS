using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Api.Wrappers;
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
                return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));

            var result = await _renewalService.ProcessPolicyRenewalAsync(customerId, id, request);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Policy renewal processed successfully."));
        }


        [HttpGet("renewals/{id}")]
        public async Task<IActionResult> GetRenewalDetails(long id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out long customerId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));


            // Pass the policyId (id) and customerId to the service
            var result = await _renewalService.GetRenewalDetailsAsync(id, customerId);

            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Renewal record not found or unauthorized."));

            return Ok(ApiResponse<object>.SuccessResponse(result, "Renewal details retrieved successfully."));
        }
    }
}