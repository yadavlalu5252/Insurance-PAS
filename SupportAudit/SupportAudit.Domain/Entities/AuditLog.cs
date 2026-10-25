
namespace SupportAudit.Domain.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }

        public string? ActorUserId { get; set; }
        public string? ActorRole { get; set; }

        public string ModuleName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? IpAddress { get; set; }

        public DateTime? LoginTimeUtc { get; set; }
        public DateTime? LogoutTimeUtc { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string Outcome { get; set; } = "Success";
        public string SourceService { get; set; } = "SupportAudit";
        public string? CorrelationId { get; set; }

        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    }
}
