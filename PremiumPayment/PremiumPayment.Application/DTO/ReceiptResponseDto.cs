using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumPayment.Application.DTO
{
    public class ReceiptResponseDto
    {
        public int receipt_id { get; set; }

        public int payment_id { get; set; }

        public string receipt_number { get; set; } = string.Empty;

        public DateTime receipt_date { get; set; }

        public string? receipt_path { get; set; }
    }
}
