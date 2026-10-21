using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class PolicyInfoDTO
    {
        public long policy_id { get; set; }
        public long customer_id { get; set; }
        public DateTime effective_date { get; set; }
        public DateTime expiry_date { get; set; }
        public string policy_status { get; set; } = string.Empty;
    }
}
