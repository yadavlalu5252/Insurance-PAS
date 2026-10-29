using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Api.Wrappers;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Infrastructure.Config;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/v1/service-requests")]
public class ServiceRequestController : ControllerBase
{
    private readonly IServiceRequestService service;


    public ServiceRequestController(IServiceRequestService service)
    {
        this.service = service;

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EndorsementRequestDTO request)
    {

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);


        if (long.TryParse(userIdClaim, out long tokenCustomerId))
        {
            request.CustomerId = tokenCustomerId;
        }
        else
        {
            return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));
        }
        var result = await service.CreateServiceRequestAsync(request);
        return Ok(ApiResponse<object>.SuccessResponse(
           new { requestNumber = result },
           "Service request created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);


        if (userRole == "Customer" && long.TryParse(userIdClaim, out long customerId))
        {
            var myRequests = await service.GetRequestsByCustomerIdAsync(customerId);
            return Ok(ApiResponse<object>.SuccessResponse(myRequests, "Service requests retrieved successfully."));
        }


        var allRequests = await service.ListRequestsAsync();
        return Ok(ApiResponse<object>.SuccessResponse(allRequests, "Service requests retrieved successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> View(long id)
    {
        var result = await service.ViewRequestAsync(id);
        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Service request not found."));

        return Ok(ApiResponse<object>.SuccessResponse(result, "Service request retrieved successfully."));

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromQuery] string newStatus)
    {

        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(adminIdClaim, out long adminId))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Admin ID missing."));


        var success = await service.UpdateRequestAsync(id, newStatus, adminId);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update service request."));

        return Ok(ApiResponse<object>.SuccessResponse(new { id, newStatus }, "Service request updated successfully."));
    }

    [HttpGet("my-policies")]
    public async Task<IActionResult> GetMyPolicies()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out long customerId))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing."));


        var policies = await service.GetCustomerPoliciesAsync(customerId);
        return Ok(ApiResponse<object>.SuccessResponse(policies, "Policies retrieved successfully."));
    }


    [HttpGet("policies/{policyId}/coverages")]
    public async Task<IActionResult> GetCoveragesForPolicy(long policyId)
    {

        var coverages = await service.GetCoveragesForPolicyAsync(policyId);
        return Ok(ApiResponse<object>.SuccessResponse(coverages, "Coverages retrieved successfully."));
    }

    [HttpGet("policies/{policyId}/riders")]
    public async Task<IActionResult> GetRidersForPolicy(long policyId)
    {

        var riders = await service.GetRidersForPolicyAsync(policyId);
        return Ok(ApiResponse<object>.SuccessResponse(riders, "Riders retrieved successfully."));
    }
}