namespace AgentManagement.Application.DTO
{
    public class AgentCommissionDTO
    {
        public long AgentId { get; set; }
        public long PolicyId { get; set; }
        public long ProductId { get; set; }
        public decimal PremiumAmount { get; set; }
    }
}