
using System.ComponentModel.DataAnnotations;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyEndorsement
    {
        [Key]
        public long endorsement_id { get; set; }

        public long policy_id { get; set; }

        public string endorsement_number { get; set; } = string.Empty;

        public string endorsement_type { get; set; } = string.Empty;

        public string old_values { get; set; } = string.Empty;

        public string new_values { get; set; } = string.Empty;

        public string status { get; set; } = "Pending";

        public string? approved_by { get; set; }
        public DateTime created_date { get; set; }

        public DateTime? approved_date { get; set; }
    }
}
