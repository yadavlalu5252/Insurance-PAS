
using SupportAudit.Application.DTOs;
using SupportAudit.Domain.Entities;

namespace SupportAudit.Application.Interfaces;

public interface IAuditService
{
    Task<List<AuditLog>> GetAllAsync();

    Task<AuditLog?> GetByIdAsync(long id);

    Task<AuditLog> CreateAsync(
        CreateAuditEventDto dto,
        string? requestIp,
        string sourceService);
}
