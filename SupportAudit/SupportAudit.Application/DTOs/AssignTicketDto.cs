using System.ComponentModel.DataAnnotations;

namespace SupportAudit.Application.DTOs;

public class AssignTicketDto
{
    [Required]
    [StringLength(100)]
    public string AssignedToUserId { get; set; } = string.Empty;
}