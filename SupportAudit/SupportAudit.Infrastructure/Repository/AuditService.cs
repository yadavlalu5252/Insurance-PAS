using Microsoft.EntityFrameworkCore;
using SupportAudit.Application.DTOs;
using SupportAudit.Application.Interfaces;
using SupportAudit.Domain.Entities;
using SupportAudit.Infrastructure.Data;

namespace SupportAudit.Infrastructure.Repository;

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;

    public AuditService(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<AuditLog>> GetAllAsync()
    {
        return _db.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(500)
            .ToListAsync();
    }

    public Task<AuditLog?> GetByIdAsync(long id)
    {
        return _db.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AuditLog> CreateAsync(
        CreateAuditEventDto dto,
        string? requestIp)
    {
        var log = new AuditLog
        {
            ModuleName = dto.ModuleName.Trim(),
            Action = dto.Action.Trim(),

            ActorUserId = dto.ActorUserId,
            ActorRole = dto.ActorRole,

            // Prefer the IP obtained from the HTTP request.
            IpAddress = requestIp ?? dto.IpAddress,

            LoginTimeUtc = dto.LoginTimeUtc,
            LogoutTimeUtc = dto.LogoutTimeUtc,

            OldValue = dto.OldValue,
            NewValue = dto.NewValue,

            EntityType = dto.EntityType,
            EntityId = dto.EntityId,

            Outcome = string.IsNullOrWhiteSpace(dto.Outcome)
                ? "Success"
                : dto.Outcome.Trim(),

            SourceService = "SupportAudit",

            CorrelationId = dto.CorrelationId,

            OccurredAtUtc =
                dto.OccurredAtUtc ?? DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);

        await _db.SaveChangesAsync();

        return log;
    }
}