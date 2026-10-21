using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class CustomerKycDto
    {
        public long KycId { get; set; }
        public long CustomerId { get; set; }
        public string KycNumber { get; set; } = string.Empty;
        public DateTime KycDate { get; set; }
        public string VerificationStatus { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public long? VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }
}
