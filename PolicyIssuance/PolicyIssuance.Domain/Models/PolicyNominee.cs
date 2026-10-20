
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyNominee
    {
        [Key]
        public long policy_nominee_id { get; set; }

        public long policy_id { get; set; }

        public string nominee_name { get; set; } = string.Empty;

        public string relationship { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal share_percentage { get; set; }
    }
}
