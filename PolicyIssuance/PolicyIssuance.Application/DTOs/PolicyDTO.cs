
namespace PolicyIssuance.Application.DTOs
{
    public class PolicyDTO
    {
        public long policy_id { get; set; }

        public string policy_number { get; set; } = string.Empty;

        public long customer_id { get; set; }

        public long quote_id { get; set; }

        public long product_id { get; set; }

        public long underwriting_id { get; set; }

        public DateTime issue_date { get; set; }

        public DateTime effective_date { get; set; }

        public DateTime expiry_date { get; set; }

        public decimal premium_amount { get; set; }

        public string policy_status { get; set; } = "Draft";
    }
}
