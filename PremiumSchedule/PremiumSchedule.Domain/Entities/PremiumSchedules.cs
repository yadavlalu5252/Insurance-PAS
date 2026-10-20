using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumSchedule.Domain.Entities
{
    public class PremiumSchedules
    {
        [Key]
        public int schedule_id { get; set; }

        public int policy_id { get; set; }

        public int installment_no { get; set; }

        public DateTime due_date { get; set; }

        public decimal premium_amount { get; set; }

        public decimal late_fee { get; set; }

        public string status { get; set; }

    }
}
