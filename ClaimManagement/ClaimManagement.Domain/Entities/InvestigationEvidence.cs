using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class InvestigationEvidence
    {
        [Key]
        public long evidence_id { get; set; }
        public long investigation_id { get; set; }
        public string description { get; set; } = string.Empty;
        public string file_name { get; set; } = string.Empty;   // original name, only for display
        public string file_path { get; set; } = string.Empty;   // random stored name
        public long uploaded_by { get; set; }
        public DateTime uploaded_date { get; set; }
    }
}
