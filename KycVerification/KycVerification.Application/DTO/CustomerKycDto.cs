using System;

namespace KycVerification.Application.DTO
{
    public class CustomerKycDto
    {
        public int KycId { get; set; }

        public int CustomerId { get; set; }

        public string KycNumber { get; set; } = string.Empty;

        public DateTime KycDate { get; set; }

        public string VerificationStatus { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public string? VerifiedBy { get; set; }

        public DateTime? VerifiedDate { get; set; }
    }
}