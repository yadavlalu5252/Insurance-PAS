
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportAudit.Api.Infrastructure;
using SupportAudit.Application.DTOs;
using SupportAudit.Application.Interfaces;

namespace SupportAudit.Api.Controllers;

[ApiController]
[Route("api/v1/support/tickets")]
[Authorize]
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

    private bool IsAdminOrSupport =>
        string.Equals(_user.Role, "Admin",
            StringComparison.OrdinalIgnoreCase)
        ||
        string.Equals(_user.Role, "Support",
            StringComparison.OrdinalIgnoreCase);

    [Authorize(Roles = "Customer,Admin,Support")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(_user.UserId) ||
            string.IsNullOrWhiteSpace(_user.Role))
        {
            return Unauthorized(Error(
                "User ID or role is missing from the JWT."));
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

    [Authorize(Roles = "Customer,Admin,Support")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets()
    {
        if (string.IsNullOrWhiteSpace(_user.UserId))
        {
            return Unauthorized(Error(
                "User ID is missing from the JWT."));
        }

        var tickets =
            await _service.GetMyTicketsAsync(_user.UserId);

        return Ok(OkData("Your tickets retrieved.", tickets));
    }

    [Authorize(Roles = "Admin,Support")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _service.GetAllTicketsAsync();

        return Ok(OkData("Tickets retrieved.", tickets));
    }

    [Authorize(Roles = "Customer,Admin,Support")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (string.IsNullOrWhiteSpace(_user.UserId))
        {
            return Unauthorized(Error(
                "User ID is missing from the JWT."));
        }

        var ticket = await _service.GetTicketByIdAsync(id);

        if (ticket is null)
        {
            return NotFound(Error("Ticket not found."));
        }

        if (!IsAdminOrSupport &&
            ticket.CreatedByUserId != _user.UserId &&
            ticket.AssignedToUserId != _user.UserId)
        {
            return Forbid();
        }

        return Ok(OkData("Ticket retrieved.", ticket));
    }

    [Authorize(Roles = "Admin,Support")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateTicketStatusDto dto)
    {
        if (string.IsNullOrWhiteSpace(_user.UserId) ||
            string.IsNullOrWhiteSpace(_user.Role))
        {
            return Unauthorized(Error(
                "User ID or role is missing from the JWT."));
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

    [Authorize(Roles = "Admin,Support")]
    [HttpPatch("{id:int}/assign")]
    public async Task<IActionResult> Assign(
        int id,
        [FromBody] AssignTicketDto dto)
    {
        if (string.IsNullOrWhiteSpace(_user.UserId) ||
            string.IsNullOrWhiteSpace(_user.Role))
        {
            return Unauthorized(Error(
                "User ID or role is missing from the JWT."));
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
