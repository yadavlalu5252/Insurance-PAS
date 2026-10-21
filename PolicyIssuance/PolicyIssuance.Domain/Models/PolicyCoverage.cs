
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyCoverage
    {
        [Key]
        public long coverage_id { get; set; }

        public long policy_id { get; set; }

        public string coverage_name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(15,2)")]
        public decimal sum_insured { get; set; }

        public DateTime coverage_start_date { get; set; }

        public DateTime coverage_end_date { get; set; }
    }
}
