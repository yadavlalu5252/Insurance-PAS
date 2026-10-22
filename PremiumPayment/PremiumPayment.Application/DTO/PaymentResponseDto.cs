using PremiumPayment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumPayment.Application.DTO
{
    public class PaymentResponseDto
    {
        public int payment_id { get; set; }

        public int schedule_id { get; set; }

        public int policy_id { get; set; }

        public DateTime payment_date { get; set; }

        public decimal amount_paid { get; set; }

        public PaymentMode payment_mode { get; set; }

        public string? transaction_reference { get; set; }

        public PaymentStatus payment_status { get; set; }
    }
}
