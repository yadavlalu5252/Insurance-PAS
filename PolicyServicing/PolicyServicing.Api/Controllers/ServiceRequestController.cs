using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;

[ApiController]
[Route("api/v1/service-requests")]
public class ServiceRequestController : ControllerBase
{
    private readonly IServiceRequestService _service;

    public ServiceRequestController(IServiceRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EndorsementRequestDTO request)
    {
        var result = await _service.CreateServiceRequestAsync(request);
        return Ok(new { Message = "Created", RequestNumber = result });
    }

    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _service.ListRequestsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> View(long id)
    {
        var result = await _service.ViewRequestAsync(id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromQuery] string newStatus, [FromQuery] long adminId)
    {
        var success = await _service.UpdateRequestAsync(id, newStatus, adminId);
        return success ? Ok("Updated") : BadRequest();
    }
}