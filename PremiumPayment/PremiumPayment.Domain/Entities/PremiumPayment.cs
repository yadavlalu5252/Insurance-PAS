namespace Premium.Domain.Entities
{
    public class PremiumPayment
    {
        public long PaymentId { get; set; }

        public long ScheduleId { get; set; }

        public long PolicyId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal AmountPaid { get; set; }

        public string PaymentMode { get; set; } = string.Empty;

        public string? TransactionReference { get; set; }

        public string PaymentStatus { get; set; } = "Initiated";
    }
}