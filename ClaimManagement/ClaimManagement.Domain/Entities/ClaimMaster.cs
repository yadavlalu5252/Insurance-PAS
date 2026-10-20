using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class ClaimMaster
    {
        [Key]
        public long claim_id { get; set; }

        public string claim_number { get; set; } = string.Empty;

        public long policy_id { get; set; }

        public long customer_id { get; set; }

        public string claim_type { get; set; } = string.Empty;

        public DateTime incident_date { get; set; }

        public DateTime claim_date { get; set; }

        public string description { get; set; } = string.Empty;

        public decimal estimated_loss { get; set; }

        public string claim_status { get; set; } = "Registered";

        [ForeignKey("claim_id")]
        public List<ClaimDocument> claim_documents { get; set; } = new();

        [ForeignKey("claim_id")]
        public List<ClaimHistory> claim_history { get; set; } = new();
    }
}
