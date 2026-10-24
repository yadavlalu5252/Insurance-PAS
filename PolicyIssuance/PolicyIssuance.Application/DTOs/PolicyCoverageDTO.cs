
namespace PolicyIssuance.Application.DTOs
{
    public class PolicyCoverageDTO
    {
        public long coverage_id { get; set; }

        public long policy_id { get; set; }

        public string coverage_name { get; set; } = string.Empty;

        public decimal sum_insured { get; set; }

        public DateTime coverage_start_date { get; set; }

        public DateTime coverage_end_date { get; set; }
    }
}
