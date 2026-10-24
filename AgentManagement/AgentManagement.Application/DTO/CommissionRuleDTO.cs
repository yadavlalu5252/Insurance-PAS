
using System;

namespace AgentManagement.Application.DTO
{
    public class CommissionRuleDTO
    {
        public long ProductId { get; set; }
        public decimal CommissionPercentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
    }
}
