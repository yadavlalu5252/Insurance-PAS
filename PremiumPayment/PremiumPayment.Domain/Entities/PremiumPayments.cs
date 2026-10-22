using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PremiumPayment.Domain.Entities
{
    public class PremiumPayments
    {
        [Key]
        public int payment_id { get; set; }

        [Required]
        public int schedule_id { get; set; }

        [Required]
        public int policy_id { get; set; }

        public DateTime payment_date { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal amount_paid { get; set; }

        [Required]
        public PaymentMode payment_mode { get; set; }

        [MaxLength(100)]
        public string? transaction_reference { get; set; }

        [Required]
        public PaymentStatus payment_status { get; set; }
    }

    public enum PaymentMode
    {
        UPI,
        CreditCard,
        DebitCard,
        NetBanking,
        Cash,
        Cheque,
        DD,
        Wallet
    }

    public enum PaymentStatus
    {
        Initiated,
        Success,
        Failed,
        Pending,
        Refunded,
        Cancelled
    }
}