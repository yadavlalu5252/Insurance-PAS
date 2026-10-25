using PremiumSchedule.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PremiumSchedule.Application.DTO
{
    public class VerifyPaymentDto
    {
        [Required]
        public PaymentStatus payment_status { get; set; }

        public string? transaction_reference { get; set; }
    }
}