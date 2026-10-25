using System;

namespace KycVerification.Application.DTO
{
    public class KycDocumentDto
    {
        public int DocumentId { get; set; }

        public int KycId { get; set; }

        public string? DocumentType { get; set; }

        public string? DocumentNumber { get; set; }

        public string? FilePath { get; set; }

        public DateTime UploadDate { get; set; }

        public string? VerificationStatus { get; set; }
    }
}