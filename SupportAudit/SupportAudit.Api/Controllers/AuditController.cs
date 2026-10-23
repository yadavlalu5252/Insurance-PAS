using Microsoft.AspNetCore.Mvc;
using SupportAudit.Application.DTOs;
using SupportAudit.Application.Interfaces;

namespace SupportAudit.Api.Controllers;

[ApiController]
[Route("api/v1/audit-logs")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _service;

    public AuditController(IAuditService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _service.GetAllAsync();

        return Ok(new
        {
            success = true,
            message = "Audit logs retrieved.",
            data = logs,
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var log = await _service.GetByIdAsync(id);

        if (log is null)
        {
            return NotFound(new
            {
                success = false,
                message = "Audit log not found.",
                data = (object?)null,
                timestamp = DateTime.UtcNow
            });
        }

        return Ok(new
        {
            success = true,
            message = "Audit log retrieved.",
            data = log,
            timestamp = DateTime.UtcNow
        });
    }

    // Protect this endpoint with service-to-service authentication
    // before integrating with other microservices.
    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent(
        [FromBody] CreateAuditEventDto dto)
    {
        var requestIp =
            HttpContext.Connection.RemoteIpAddress?.ToString();

        var log = await _service.CreateAsync(dto, requestIp);

        return CreatedAtAction(
            nameof(GetById),
            new { id = log.Id },
            new
            {
                success = true,
                message = "Audit event recorded.",
                data = log,
                timestamp = DateTime.UtcNow
            });
    }
}