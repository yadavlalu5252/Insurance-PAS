using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Api.Wrappers;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System.Security.Claims;

namespace PolicyServicing.API.Controllers;

[ApiController]
[Route("api/v1")]
[Authorize]
public class PolicyRenewalController : ControllerBase
{
    private readonly IPolicyRenew renewalService;

    public PolicyRenewalController(IPolicyRenew renewalService)
    {
        this.renewalService = renewalService;
    }

    [HttpPost("policies/{id:long}/renew")]
    public async Task<IActionResult> RenewPolicy(long id, [FromBody] RenewalRequestDTO request)
    {
        if (request == null)
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid renewal payload."));

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out long customerId))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));

        var result = await renewalService.ProcessPolicyRenewalAsync(customerId, id, request);

        return Ok(ApiResponse<string>.SuccessResponse(result, "Policy renewal processed successfully."));
    }

    [HttpGet("renewals/{id:long}")]
    public async Task<IActionResult> GetRenewalDetails(long id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out long customerId))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));

        var result = await renewalService.GetRenewalDetailsAsync(id, customerId);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Renewal record not found or unauthorized."));

     
        return Ok(ApiResponse<object>.SuccessResponse(result, "Renewal details retrieved successfully."));
    }
}