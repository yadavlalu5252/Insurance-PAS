using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class PolicyDTO
    {
        public long policy_id { get; set; }
        public long customer_id { get; set; }
        public long product_id { get; set; }
        public long? agent_id { get; set; }
        public long? branch_id { get; set; }
        public string? policy_number { get; set; }
        public string? status { get; set; }
        public decimal? premium_amount { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}
