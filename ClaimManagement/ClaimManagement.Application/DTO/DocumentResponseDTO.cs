using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class DocumentResponseDTO
    {
        public long document_id { get; set; }
        public long claim_id { get; set; }
        public string document_name { get; set; } = string.Empty;
        public string verification_status { get; set; } = string.Empty;
        public DateTime uploaded_date { get; set; }
    }
}
