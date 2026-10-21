using System;
using System.Collections.Generic;
using System.Text;

namespace KycVerification.Application.DTO
{
    public class CkycDetailsDto
    {
        public long CkycId { get; set; }
        public long CustomerId { get; set; }
        public string? CkycNumber { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string? CkycStatus { get; set; }
    }
}
