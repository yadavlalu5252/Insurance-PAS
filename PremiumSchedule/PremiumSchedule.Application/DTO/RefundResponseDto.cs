using PremiumSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class RefundResponseDto
    {
        public int refund_id { get; set; }

        public int payment_id { get; set; }

        public decimal refund_amount { get; set; }

        public DateTime refund_date { get; set; }

        public RefundStatus refund_status { get; set; }

        public int approved_by { get; set; }
    }
}
