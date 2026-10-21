
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PolicyIssuance.Domain.Enums;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyEntity
    {
        [Key]
        public long policy_id { get; set; }

        [Required]
        [MaxLength(50)]
        public string policy_number { get; set; } = string.Empty;

        public long customer_id { get; set; }

        public long quote_id { get; set; }

        public long product_id { get; set; }

        public long underwriting_id { get; set; }

        public DateTime issue_date { get; set; }

        public DateTime effective_date { get; set; }

        public DateTime expiry_date { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal premium_amount { get; set; }

        public PolicyStatus policy_status { get; set; }
            = PolicyStatus.Draft;
    }
}
