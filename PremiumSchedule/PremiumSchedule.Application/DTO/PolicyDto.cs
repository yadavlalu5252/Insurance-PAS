using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class PolicyDto
    {
        public int policy_id { get; set; }

        public int quote_id { get; set; }

        public DateTime issue_date { get; set; }

        public DateTime effective_date { get; set; }

        public DateTime expiry_date { get; set; }

        public decimal premium_amount { get; set; }

        public string? policy_status { get; set; }
    }
}
