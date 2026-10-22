using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class InvestigationMaster
    {
        [Key]
        public long investigation_id { get; set; }
        public string investigation_number { get; set; } = string.Empty;   // recommendation
        public long claim_id { get; set; }
        public long investigator_id { get; set; }     // employee id from another module, not validated yet
        public string investigation_reason { get; set; } = string.Empty;
        public DateTime assigned_date { get; set; }
        public string investigation_status { get; set; } = "Pending";      // Pending / Completed (BRD)
        public string? findings { get; set; }                              // recommendation
        public string? recommendation { get; set; }                        // Genuine or Fraud (recommendation)
        public string? report_path { get; set; }                           // stored file name, never a public path
        public DateTime? completed_date { get; set; }

        [Timestamp]
        public byte[] row_version { get; set; } = Array.Empty<byte>();
    }
}
