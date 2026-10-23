using System;

namespace KycVerification.Application.DTO
{
    public class CkycDetailsDto
    {
        public int CkycId { get; set; }

        public int CustomerId { get; set; }

        public string? CkycNumber { get; set; }

        public DateTime? VerificationDate { get; set; }

        public string? CkycStatus { get; set; }
    }
}