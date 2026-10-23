using SupportAudit.Application.DTOs;
using SupportAudit.Domain.Entities;

namespace SupportAudit.Application.Interfaces;

public interface ISupportService
{
    Task<SupportTicket> CreateTicketAsync(
        CreateTicketDto dto,
        string userId,
        string userRole,
        string? ipAddress);

    Task<List<SupportTicket>> GetMyTicketsAsync(string userId);

    Task<SupportTicket?> GetTicketByIdAsync(int id);

    Task<List<SupportTicket>> GetAllTicketsAsync();

    Task<SupportTicket?> UpdateStatusAsync(
        int id,
        UpdateTicketStatusDto dto,
        string actorUserId,
        string actorRole,
        string? ipAddress);

    Task<SupportTicket?> AssignTicketAsync(
        int id,
        string assignedToUserId,
        string actorUserId,
        string actorRole,
        string? ipAddress);
}