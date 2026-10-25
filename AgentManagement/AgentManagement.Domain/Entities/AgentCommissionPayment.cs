using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgentManagement.Domain.Entities
{
    public class AgentCommissionPayment
    {
        [Key]
        public long CommissionPaymentId { get; set; }

        public long AgentId { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
    }
}
