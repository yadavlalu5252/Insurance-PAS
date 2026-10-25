
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportAudit.Application.DTOs;
using SupportAudit.Application.Interfaces;

namespace SupportAudit.Api.Controllers;

[ApiController]
[Route("api/v1/audit-logs")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _service;
    private readonly IConfiguration _configuration;

    public AuditController(
        IAuditService service,
        IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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

    [AllowAnonymous]
    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent(
        [FromBody] CreateAuditEventDto dto)
    {
        var serviceName =
            Request.Headers["X-Service-Name"].ToString();

        var suppliedKey =
            Request.Headers["X-Service-Api-Key"].ToString();

        if (string.IsNullOrWhiteSpace(serviceName) ||
            string.IsNullOrWhiteSpace(suppliedKey))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Service credentials are required."
            });
        }

        // The service name must exist in trusted server configuration.
        var expectedKey = _configuration[
            $"Audit:ServiceApiKeys:{serviceName}"];

        if (string.IsNullOrWhiteSpace(expectedKey) ||
            !SecureEquals(suppliedKey, expectedKey))
        {
            return Unauthorized(new
            {
                success = false,
                message = "Invalid service credentials."
            });
        }

        
        var requestIp =
            HttpContext.Connection.RemoteIpAddress?.ToString();

        var log = await _service.CreateAsync(
            dto,
            requestIp,
            serviceName);

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

    private static bool SecureEquals(string supplied, string expected)
    {
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return suppliedBytes.Length == expectedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(
                   suppliedBytes, expectedBytes);
    }
}
