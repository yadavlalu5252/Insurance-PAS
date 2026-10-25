using System;
using System.Collections.Generic;
using System.Text;

namespace SupportAudit.Domain.Entities
{
    public class SupportTicket
    {
        public int Id { get; set; }

        public string TicketNumber { get; set; } = string.Empty;

        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByRole { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";

        public string? AssignedToUserId { get; set; }
        public string? Resolution { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAtUtc { get; set; }
    }
}
