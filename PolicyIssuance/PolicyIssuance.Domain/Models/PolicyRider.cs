
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyRider
    {
        [Key]
        public long rider_id { get; set; }

        public long policy_id { get; set; }

        public string rider_name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(15,2)")]
        public decimal premium_amount { get; set; }
    }
}
