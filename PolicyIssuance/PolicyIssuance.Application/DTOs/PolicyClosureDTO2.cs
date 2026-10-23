namespace PolicyIssuance.Application.DTOs
{
    public class PolicyClosureDTO2
    {
        public long policy_closure_id { get; set; }

        public long policy_id { get; set; }

        public string closure_reason { get; set; } = string.Empty;

        public string requested_by { get; set; } = string.Empty;

        public DateTime requested_at { get; set; }

        public string closure_status { get; set; } = string.Empty;

        public string? approved_by { get; set; }

        public DateTime? approved_at { get; set; }

        public string? rejection_reason { get; set; }

        public DateTime? closed_at { get; set; }

        public decimal? refund_amount { get; set; }

        public string? remarks { get; set; }
    }
}
