using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class ClaimCreateDto
    {
        public long policy_id { get; set; }

        public long customer_id { get; set; }

        public string claim_type { get; set; } = string.Empty;

        public DateTime incident_date { get; set; }

        public string description { get; set; } = string.Empty;

        public decimal estimated_loss { get; set; }
    }
}
