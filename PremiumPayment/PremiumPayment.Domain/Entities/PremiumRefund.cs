using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PremiumPayment.Domain.Entities
{
    public class PremiumRefund
    {
        [Key]
        public long refund_id { get; set; }

        [Required]
        public long payment_id { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal refund_amount { get; set; }

        public DateTime refund_date { get; set; }

        [Required]
        public RefundStatus refund_status { get; set; }

        [Required]
        public long approved_by { get; set; }
    }

    public enum RefundStatus
    {
        Pending,
        Approved,
        Completed,
        Rejected
    }
}
