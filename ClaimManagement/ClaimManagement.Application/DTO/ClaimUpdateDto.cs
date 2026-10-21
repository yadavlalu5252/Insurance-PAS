using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class ClaimUpdateDto
    {
        public string description { get; set; } = string.Empty;

        public decimal estimated_loss { get; set; }
    }
}
