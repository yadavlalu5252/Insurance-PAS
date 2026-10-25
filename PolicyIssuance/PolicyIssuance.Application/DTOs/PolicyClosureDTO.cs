
namespace PolicyIssuance.Application.DTOs
{ 
    public class PolicyClosureDTO
    {
        public long policy_closure_id { get; set; }

        public long policy_id { get; set; }

        public string closure_reason { get; set; } = string.Empty;

        public string requested_by { get; set; } = string.Empty;

        public string? remarks { get; set; }
    }
}
