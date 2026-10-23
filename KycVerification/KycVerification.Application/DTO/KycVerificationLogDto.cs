using System;

namespace KycVerification.Application.DTO
{
    public class KycVerificationLogDto
    {
        public int LogId { get; set; }

        public int KycId { get; set; }

        public string Action { get; set; } = string.Empty;

        public int? PerformedBy { get; set; }

        public DateTime ActionDate { get; set; }

        public string? Remarks { get; set; }
    }
}