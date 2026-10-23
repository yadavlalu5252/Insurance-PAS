using System.ComponentModel.DataAnnotations;

namespace SupportAudit.Application.DTOs;

public class CreateAuditEventDto
{
    [Required]
    [StringLength(100)]
    public string ModuleName { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Action { get; set; } = string.Empty;

    public string? ActorUserId { get; set; }

    public string? ActorRole { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? LoginTimeUtc { get; set; }

    public DateTime? LogoutTimeUtc { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public string Outcome { get; set; } = "Success";

    public string? CorrelationId { get; set; }

    public DateTime? OccurredAtUtc { get; set; }
}