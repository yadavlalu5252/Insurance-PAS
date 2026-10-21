using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class KycVerificationLogDto
    {
        public long LogId { get; set; }
        public long KycId { get; set; }
        public string Action { get; set; } = string.Empty;
        public long? PerformedBy { get; set; }
        public DateTime ActionDate { get; set; }
        public string? Remarks { get; set; }
    }
}
