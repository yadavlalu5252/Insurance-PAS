
namespace PolicyIssuance.Application.DTOs
{
    public class PolicyHistoryDTO
    {
        public long policy_history_id { get; set; }

        public long policy_id { get; set; }

        public string action { get; set; } = string.Empty;

        public string performed_by { get; set; } = string.Empty;

        public DateTime performed_date { get; set; }

        public string remarks { get; set; } = string.Empty;
    }
}
