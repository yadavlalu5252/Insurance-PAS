using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumSchedule.Domain.Entities
{
    public class PaymentReceipt
    {
        [Key]
        public int receipt_id { get; set; }

        [Required]
        public int payment_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string receipt_number { get; set; } = string.Empty;

        public DateTime receipt_date { get; set; }

        [MaxLength(500)]
        public string? receipt_path { get; set; }
    }
}
