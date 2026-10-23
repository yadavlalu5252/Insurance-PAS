using Microsoft.AspNetCore.Mvc;
using SupportAudit.Api.Infrastructure;
using SupportAudit.Application.DTOs;
using SupportAudit.Application.Interfaces;

namespace SupportAudit.Api.Controllers;

[ApiController]
[Route("api/v1/support/tickets")]
public class SupportController : ControllerBase
{
    private readonly ISupportService _service;
    private readonly RequestUserContext _user;

    public SupportController(
        ISupportService service,
        RequestUserContext user)
    {
        _service = service;
        _user = user;
    }

    private bool HasUser =>
        !string.IsNullOrWhiteSpace(_user.UserId);

    private bool IsAdmin =>
        string.Equals(
            _user.Role,
            "Admin",
            StringComparison.OrdinalIgnoreCase)
        ||
        string.Equals(
            _user.Role,
            "Support",
            StringComparison.OrdinalIgnoreCase);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTicketDto dto)
    {
        if (!HasUser)
        {
            return BadRequest(
                Error("Provide X-User-Id header for local testing."));
        }

        if (string.IsNullOrWhiteSpace(_user.Role))
        {
            return BadRequest(
                Error("Provide X-User-Role header for local testing."));
        }

        var ticket = await _service.CreateTicketAsync(
            dto,
            _user.UserId,
            _user.Role,
            _user.IpAddress);

        return CreatedAtAction(
            nameof(GetById),
            new { id = ticket.Id },
            OkData("Ticket created.", ticket));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets()
    {
        if (!HasUser)
        {
            return BadRequest(
                Error("Provide X-User-Id header for local testing."));
        }

        var tickets =
            await _service.GetMyTicketsAsync(_user.UserId);

        return Ok(OkData("Your tickets retrieved.", tickets));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAdmin)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                Error("Admin or Support role required."));
        }

        var tickets = await _service.GetAllTicketsAsync();

        return Ok(OkData("Tickets retrieved.", tickets));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!HasUser)
        {
            return BadRequest(
                Error("Provide X-User-Id header for local testing."));
        }

        var ticket = await _service.GetTicketByIdAsync(id);

        if (ticket is null)
        {
            return NotFound(Error("Ticket not found."));
        }

        if (!IsAdmin
            && ticket.CreatedByUserId != _user.UserId
            && ticket.AssignedToUserId != _user.UserId)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                Error("You cannot view this ticket."));
        }

        return Ok(OkData("Ticket retrieved.", ticket));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateTicketStatusDto dto)
    {
        if (!IsAdmin)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                Error("Admin or Support role required."));
        }

        var ticket = await _service.UpdateStatusAsync(
            id,
            dto,
            _user.UserId,
            _user.Role,
            _user.IpAddress);

        if (ticket is null)
        {
            return NotFound(Error("Ticket not found."));
        }

        return Ok(OkData("Ticket status updated.", ticket));
    }

    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(
        int id,
        [FromBody] AssignTicketDto dto)
    {
        if (!IsAdmin)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                Error("Admin or Support role required."));
        }

        var ticket = await _service.AssignTicketAsync(
            id,
            dto.AssignedToUserId,
            _user.UserId,
            _user.Role,
            _user.IpAddress);

        if (ticket is null)
        {
            return NotFound(Error("Ticket not found."));
        }

        return Ok(OkData("Ticket assigned.", ticket));
    }

    private object OkData(string message, object data)
    {
        return new
        {
            success = true,
            message,
            data,
            timestamp = DateTime.UtcNow
        };
    }

    private object Error(string message)
    {
        return new
        {
            success = false,
            message,
            data = (object?)null,
            timestamp = DateTime.UtcNow
        };
    }
}