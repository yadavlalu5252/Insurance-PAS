using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class VerifyDocumentDTO
    {
        public long document_id { get; set; }

        // "Verified" or "Rejected"
        public string verification_status { get; set; } = string.Empty;

        public string? remarks { get; set; }
    }
}
