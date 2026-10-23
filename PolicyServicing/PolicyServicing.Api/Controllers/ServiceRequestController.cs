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
        // 🚨 1. Get the real Customer ID from the Token
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 🚨 2. If it's a valid token, override the JSON body!
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
        var result = await service.ListRequestsAsync();
        return Ok(result);
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
        // 🚨 3. Get the Admin ID from the Token, don't trust the URL!
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(adminIdClaim, out long adminId))
            return Unauthorized("Admin ID missing.");

        var success = await service.UpdateRequestAsync(id, newStatus, adminId);
        return success ? Ok("Updated") : BadRequest();
    }
}