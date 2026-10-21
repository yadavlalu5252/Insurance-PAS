
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyClosure
    {
        [Key]
        public long policy_closure_id { get; set; }

        public long policy_id { get; set; }

        public string closure_reason { get; set; } = string.Empty;

        public string requested_by { get; set; } = string.Empty;

        public DateTime requested_at { get; set; } = DateTime.UtcNow;

        public string closure_status { get; set; } = "Pending";

        public string? approved_by { get; set; }

        public DateTime? approved_at { get; set; }

        public string? rejection_reason { get; set; }

        public DateTime? closed_at { get; set; }

        public decimal? refund_amount { get; set; }

        public string? remarks { get; set; }
    }
}
