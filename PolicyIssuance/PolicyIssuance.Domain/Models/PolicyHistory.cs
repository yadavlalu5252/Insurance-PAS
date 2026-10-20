
using System.ComponentModel.DataAnnotations;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyHistory
    {
        [Key]
        public long policy_history_id { get; set; }

        public long policy_id { get; set; }

        public string action { get; set; } = string.Empty;

        public string performed_by { get; set; } = string.Empty;

        public DateTime performed_date { get; set; }

        public string remarks { get; set; } = string.Empty;
    }
}
