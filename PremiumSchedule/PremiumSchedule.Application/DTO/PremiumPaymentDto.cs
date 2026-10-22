namespace Premium.Application.DTOs
{
    public class PremiumPaymentDto
    {
        public long ScheduleId { get; set; }

        public long PolicyId { get; set; }

        public decimal AmountPaid { get; set; }

        public string PaymentMode { get; set; } = string.Empty;

        public string? TransactionReference { get; set; }
    }
}