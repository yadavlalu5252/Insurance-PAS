using System.ComponentModel.DataAnnotations;

namespace SupportAudit.Application.DTOs;

public class UpdateTicketStatusDto
{
    [Required]
    [RegularExpression(
        "^(Open|InProgress|Resolved|Closed)$",
        ErrorMessage =
            "Status must be Open, InProgress, Resolved, or Closed.")]
    public string Status { get; set; } = "Open";

    [StringLength(2000)]
    public string? Resolution { get; set; }
}