using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class GeneratePremiumScheduleDto
    {
        public int policy_id { get; set; }

        public DateTime issue_date { get; set; }

        public DateTime effective_date { get; set; }

        public DateTime expiry_date { get; set; }

        public string premium_frequency { get; set; }

        public decimal total_premium { get; set; }
    }
}
