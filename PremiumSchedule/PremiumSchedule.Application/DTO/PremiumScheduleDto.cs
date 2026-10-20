using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class PremiumScheduleDto
    {
        public int schedule_id { get; set; }

        public int policy_id { get; set; }

        public int installment_no { get; set; }

        public DateTime due_date { get; set; }

        public decimal premium_amount { get; set; }

        public decimal late_fee { get; set; }

        public string status { get; set; }
    }
}
