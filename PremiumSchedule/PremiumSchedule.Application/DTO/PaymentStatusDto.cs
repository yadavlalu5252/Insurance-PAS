using PremiumSchedule.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumSchedule.Application.DTO
{
    public class PaymentStatusDto
    {
        [Required]
        public PaymentStatus payment_status { get; set; }

        public string? transaction_reference { get; set; }
    }
}
