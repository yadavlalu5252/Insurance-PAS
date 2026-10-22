using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class EvidenceResponseDTO
    {
        public long evidence_id { get; set; }
        public long investigation_id { get; set; }
        public string description { get; set; } = string.Empty;
        public string file_name { get; set; } = string.Empty;
        public long uploaded_by { get; set; }
        public DateTime uploaded_date { get; set; }
    }
}
