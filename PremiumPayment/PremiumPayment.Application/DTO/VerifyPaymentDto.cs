using PremiumPayment.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Premium.Application.DTOs
{
    public class VerifyPaymentDto
    {
        [Required]
        public PaymentStatus payment_status { get; set; }

        public string? transaction_reference { get; set; }
    }
}