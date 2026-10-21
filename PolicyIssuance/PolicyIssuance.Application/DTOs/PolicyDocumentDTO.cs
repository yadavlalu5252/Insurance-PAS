
namespace PolicyIssuance.Application.DTOs
{
    public class PolicyDocumentDTO
    {
        public long policy_document_id { get; set; }

        public long policy_id { get; set; }

        public string document_name { get; set; } = string.Empty;

        public string document_path { get; set; } = string.Empty;

        public int version { get; set; }

        public DateTime generated_date { get; set; }
    }
}
