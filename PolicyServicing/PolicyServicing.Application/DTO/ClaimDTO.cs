using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class ClaimDTO
    {
        public long claim_id { get; set; }
        public string claim_number { get; set; }
        public long policy_id { get; set; }
        public long customer_id { get; set; }
        public string claim_type { get; set; }
        public DateOnly incident_date { get; set; }
        public DateOnly claim_date { get; set; }
        public decimal estimated_loss { get; set; }
        public string claim_status { get; set; }
    }
}
