using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SupportAudit.Application.DTOs;
using SupportAudit.Application.Interfaces;
using SupportAudit.Domain.Entities;
using SupportAudit.Infrastructure.Data;

namespace SupportAudit.Infrastructure.Repository;

public class SupportService : ISupportService
{
    private readonly AppDbContext _db;

    public SupportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SupportTicket> CreateTicketAsync(
        CreateTicketDto dto,
        string userId,
        string userRole,
        string? ipAddress)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;

        var ticket = new SupportTicket
        {
            TicketNumber =
                $"SUP-{now:yyyyMMddHHmmss}-" +
                $"{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",

            CreatedByUserId = userId,
            CreatedByRole = userRole,

            Subject = dto.Subject.Trim(),
            Description = dto.Description.Trim(),
            Category = dto.Category.Trim(),
            Priority = dto.Priority,

            Status = "Open",
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        _db.SupportTickets.Add(ticket);

        await _db.SaveChangesAsync();

        AddAudit(
            "CreateTicket",
            userId,
            userRole,
            ipAddress,
            null,
            ticket,
            ticket.Id);

        await _db.SaveChangesAsync();

        await transaction.CommitAsync();

        return ticket;
    }

    public Task<List<SupportTicket>> GetMyTicketsAsync(
        string userId)
    {
        return _db.SupportTickets
            .AsNoTracking()
            .Where(x => x.CreatedByUserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }

    public Task<SupportTicket?> GetTicketByIdAsync(int id)
    {
        return _db.SupportTickets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<SupportTicket>> GetAllTicketsAsync()
    {
        return _db.SupportTickets
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task<SupportTicket?> UpdateStatusAsync(
        int id,
        UpdateTicketStatusDto dto,
        string actorUserId,
        string actorRole,
        string? ipAddress)
    {
        var ticket = await _db.SupportTickets
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ticket is null)
        {
            return null;
        }

        var oldValue = new
        {
            ticket.Status,
            ticket.Resolution
        };

        ticket.Status = dto.Status;
        ticket.Resolution = dto.Resolution;
        ticket.UpdatedAtUtc = DateTime.UtcNow;

        ticket.ResolvedAtUtc =
            dto.Status is "Resolved" or "Closed"
                ? DateTime.UtcNow
                : null;

        AddAudit(
            "UpdateTicketStatus",
            actorUserId,
            actorRole,
            ipAddress,
            oldValue,
            new
            {
                ticket.Status,
                ticket.Resolution
            },
            ticket.Id);

        await _db.SaveChangesAsync();

        return ticket;
    }

    public async Task<SupportTicket?> AssignTicketAsync(
        int id,
        string assignedToUserId,
        string actorUserId,
        string actorRole,
        string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(assignedToUserId))
        {
            throw new ArgumentException(
                "AssignedToUserId is required.");
        }

        var ticket = await _db.SupportTickets
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ticket is null)
        {
            return null;
        }

        var oldAssignedTo = ticket.AssignedToUserId;

        ticket.AssignedToUserId = assignedToUserId.Trim();
        ticket.UpdatedAtUtc = DateTime.UtcNow;

        AddAudit(
            "AssignTicket",
            actorUserId,
            actorRole,
            ipAddress,
            new { assignedToUserId = oldAssignedTo },
            new { assignedToUserId = ticket.AssignedToUserId },
            ticket.Id);

        await _db.SaveChangesAsync();

        return ticket;
    }

    private void AddAudit(
        string action,
        string actorUserId,
        string actorRole,
        string? ipAddress,
        object? oldValue,
        object? newValue,
        int ticketId)
    {
        var audit = new AuditLog
        {
            ActorUserId = actorUserId,
            ActorRole = actorRole,

            ModuleName = "Support",
            Action = action,
            IpAddress = ipAddress,

            OldValue = oldValue is null
                ? null
                : JsonSerializer.Serialize(oldValue),

            NewValue = newValue is null
                ? null
                : JsonSerializer.Serialize(newValue),

            EntityType = nameof(SupportTicket),
            EntityId = ticketId.ToString(),

            Outcome = "Success",
            SourceService = "SupportAudit",
            OccurredAtUtc = DateTime.UtcNow
        };

        _db.AuditLogs.Add(audit);
    }
}