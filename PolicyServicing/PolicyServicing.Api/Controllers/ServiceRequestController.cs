using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
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

        var result = await service.CreateServiceRequestAsync(request);
        return Ok(new { Message = "Created", RequestNumber = result });
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        // 1. Get User Role and ID from JWT
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 2. If Customer, fetch only their requests
        if (userRole == "Customer" && long.TryParse(userIdClaim, out long customerId))
        {
            var myRequests = await service.GetRequestsByCustomerIdAsync(customerId);
            return Ok(myRequests);
        }

        // 3. If Admin/Agent, fetch all
        var allRequests = await service.ListRequestsAsync();
        return Ok(allRequests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> View(long id)
    {
        var result = await service.ViewRequestAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromQuery] string newStatus)
    {
       
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(adminIdClaim, out long adminId))
            return Unauthorized("Admin ID missing.");

        var success = await service.UpdateRequestAsync(id, newStatus, adminId);
        return success ? Ok("Updated") : BadRequest();
    }

    [HttpGet("my-policies")]
    public async Task<IActionResult> GetMyPolicies()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdClaim, out long customerId))
            return Unauthorized("Customer ID missing.");


        var policies = await service.GetCustomerPoliciesAsync(customerId, token);
        return Ok(policies);
    }
}