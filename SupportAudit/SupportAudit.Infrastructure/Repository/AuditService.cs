
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
        string? requestIp,
        string sourceService)
    {
        if (string.IsNullOrWhiteSpace(dto.ModuleName) ||
            string.IsNullOrWhiteSpace(dto.Action))
        {
            throw new ArgumentException(
                "ModuleName and Action are required.");
        }

        var log = new AuditLog
        {
            ModuleName = dto.ModuleName.Trim(),
            Action = dto.Action.Trim(),

            ActorUserId = dto.ActorUserId,
            ActorRole = dto.ActorRole,

            IpAddress = requestIp,

            LoginTimeUtc = dto.LoginTimeUtc,
            LogoutTimeUtc = dto.LogoutTimeUtc,

            OldValue = dto.OldValue,
            NewValue = dto.NewValue,

            EntityType = dto.EntityType,
            EntityId = dto.EntityId,

            Outcome = string.IsNullOrWhiteSpace(dto.Outcome)
                ? "Success"
                : dto.Outcome.Trim(),

            // Set from the service whose credentials were validated.
            SourceService = sourceService,

            CorrelationId = string.IsNullOrWhiteSpace(dto.CorrelationId)
                ? null
                : dto.CorrelationId.Trim(),

            // Set by the central audit API, not by the caller.
            OccurredAtUtc = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();

        return log;
    }
}
