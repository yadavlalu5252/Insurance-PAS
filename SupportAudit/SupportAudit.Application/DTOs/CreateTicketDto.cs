using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SupportAudit.Application.DTOs
{
    public class CreateTicketDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(3000, MinimumLength = 5)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Low|Medium|High)$")]
        public string Priority { get; set; } = "Medium";
    }
}
