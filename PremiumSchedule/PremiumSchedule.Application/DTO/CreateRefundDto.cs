using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class CreateRefundDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int payment_id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal refund_amount { get; set; }
    }
}
