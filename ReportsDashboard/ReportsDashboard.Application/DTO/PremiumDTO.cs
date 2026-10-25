using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class PremiumDTO
    {
        public long premium_id { get; set; }
        public long policy_id { get; set; }
        public long? customer_id { get; set; }
        public decimal amount { get; set; }
        public decimal? refund_amount { get; set; }
        public DateTime? due_date { get; set; }
        public DateTime? payment_date { get; set; }
        public string? status { get; set; }
        public string? payment_frequency { get; set; }
    }
}
