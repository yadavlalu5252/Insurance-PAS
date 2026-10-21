using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class ClaimHistory
    {
        [Key]
        public long history_id { get; set; }

        public long claim_id { get; set; }

        public string action { get; set; } = string.Empty;

        public long performed_by { get; set; }

        public DateTime action_date { get; set; }

        public string? remarks { get; set; }
    }
}
