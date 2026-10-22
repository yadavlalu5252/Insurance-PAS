using PremiumPayment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumPayment.Application.DTO
{
    public class CreatePaymentDto
    {
        [Required]
        public int schedule_id { get; set; }

        [Required]
        public int policy_id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal amount_paid { get; set; }

        [Required]
        public PaymentMode payment_mode { get; set; }

        public string? transaction_reference { get; set; }
    }
}
