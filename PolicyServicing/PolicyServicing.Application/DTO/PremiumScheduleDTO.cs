using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class PremiumScheduleDTO
    {
        public long schedule_id { get; set; }
        public long policy_id { get; set; }
        public int installment_no { get; set; }
        public DateTime due_date { get; set; }
        public decimal premium_amount { get; set; }
        public decimal late_fee { get; set; }
        public string status { get; set; }
    }
}
