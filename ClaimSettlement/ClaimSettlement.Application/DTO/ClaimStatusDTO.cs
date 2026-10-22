
namespace ClaimSettlement.Application.DTO
{
    public class ClaimStatusDTO
    {
        public long claim_id { get; set; }

        public string claim_number { get; set; } = string.Empty;

        public string claim_status { get; set; } = string.Empty;
    }
}
