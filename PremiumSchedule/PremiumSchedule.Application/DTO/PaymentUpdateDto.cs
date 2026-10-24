using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class PaymentUpdateDto
    {
        public decimal amount_paid { get; set; }

        public int payment_id { get; set; }

    }
}
