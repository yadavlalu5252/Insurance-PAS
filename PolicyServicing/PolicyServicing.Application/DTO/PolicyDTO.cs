using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class PolicyDTO
    {
        public long policy_id { get; set; }
        public string policy_number { get; set; }
        public long quote_id { get; set; }
        public long customer_id { get; set; }
        public long product_id { get; set; }
        public DateTime issue_date { get; set; }
        public DateTime effective_date { get; set; }
        public DateTime expiry_date { get; set; }
        public string policy_status { get; set; }

        public decimal premium_amount { get; set; }
    }
}
