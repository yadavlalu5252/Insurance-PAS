namespace ClaimSettlement.Application.DTO
{
    public class SettlementDTO
    {
        public decimal ApprovedAmount { get; set; }

        public string PaymentMode { get; set; } = string.Empty;

        public string SettlementStatus { get; set; } = string.Empty;
    }
}
