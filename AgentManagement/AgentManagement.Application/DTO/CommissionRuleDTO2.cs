
using System;

namespace AgentManagement.Application.DTO
{
    public class CommissionRuleDTO2
    {
        public long CommissionRuleId { get; set; }
        public long ProductId { get; set; }
        public decimal CommissionPercentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
