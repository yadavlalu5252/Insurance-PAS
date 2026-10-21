using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.DTO
{
    public class KycDocumentDto
    {
        public long DocumentId { get; set; }
        public long KycId { get; set; }
        public string? DocumentType { get; set; } 
        public string? DocumentNumber { get; set; }
        public string? FilePath { get; set; } 
        public DateTime UploadDate { get; set; }
        public string? VerificationStatus { get; set; }
    }
}
