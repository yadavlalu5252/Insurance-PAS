
namespace PolicyIssuance.Application.DTOs
{
    public class PolicyNomineeDTO
    {
        public long policy_nominee_id { get; set; }

        public long policy_id { get; set; }

        public string nominee_name { get; set; } = string.Empty;

        public string relationship { get; set; } = string.Empty;

        public decimal share_percentage { get; set; }
    }
}
