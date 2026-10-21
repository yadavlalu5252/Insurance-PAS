namespace Premium.Application.DTOs
{
    public class VerifyPaymentDto
    {
        public long PaymentId { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? TransactionReference { get; set; }

        public string VerificationSource { get; set; } = string.Empty;
    }
}