using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class InvestigationResponseDTO
    {
        public long investigation_id { get; set; }
        public string investigation_number { get; set; } = string.Empty;
        public long claim_id { get; set; }
        public long investigator_id { get; set; }
        public string investigation_reason { get; set; } = string.Empty;
        public DateTime assigned_date { get; set; }
        public string investigation_status { get; set; } = string.Empty;
        public string? findings { get; set; }
        public string? recommendation { get; set; }
        public DateTime? completed_date { get; set; }
    }
}
