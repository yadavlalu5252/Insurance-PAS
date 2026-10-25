using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class ClaimsDTO
    {
        public long claim_id { get; set; }
        public long policy_id { get; set; }
        public long? customer_id { get; set; }
        public decimal claim_amount { get; set; }
        public decimal? settlement_amount { get; set; }
        public DateTime? claim_date { get; set; }
        public DateTime? settlement_date { get; set; }
        public string? status { get; set; }
        public string? claim_type { get; set; }
        public bool? fraud_investigation { get; set; }
    }
}
