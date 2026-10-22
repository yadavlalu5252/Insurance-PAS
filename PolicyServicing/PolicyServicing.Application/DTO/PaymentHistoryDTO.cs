using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class PaymentHistoryDTO
    {
        public long payment_id { get; set; }
        public long schedule_id { get; set; }
        public long policy_id { get; set; }
        public DateTime payment_date { get; set; }
        public decimal amount_paid { get; set; }
        public string payment_mode { get; set; }
        public string transaction_reference { get; set; }
        public string payment_status { get; set; }
    }
}
