using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaimSettlement.Domain.Entities
{
    public class Settlement
    {
        [Key]
        public long SettlementId { get; set; }

        public long ClaimId { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal ApprovedAmount { get; set; }

        public string PaymentMode { get; set; } = string.Empty;

        public string? TransactionReference { get; set; }

        public DateTime? SettlementDate { get; set; }

        public string SettlementStatus { get; set; } = "Pending";
    }
}