using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class ClaimResponseDTO
    {
        public long claim_id { get; set; }
        public string claim_number { get; set; } = string.Empty;
        public long policy_id { get; set; }
        public long customer_id { get; set; }
        public string claim_type { get; set; } = string.Empty;
        public DateTime incident_date { get; set; }
        public DateTime claim_date { get; set; }
        public string description { get; set; } = string.Empty;
        public decimal estimated_loss { get; set; }
        public string claim_status { get; set; } = string.Empty;
    }
}
