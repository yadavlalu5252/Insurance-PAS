
using System.ComponentModel.DataAnnotations;

namespace PolicyIssuance.Domain.Entities
{
    public class PolicyDocument
    {
        [Key]
        public long policy_document_id { get; set; }

        public long policy_id { get; set; }

        public string document_name { get; set; } = string.Empty;

        public string document_path { get; set; } = string.Empty;

        public int version { get; set; } = 1;

        public DateTime generated_date { get; set; }
    }
}
