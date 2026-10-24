using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.DTO
{
    public class AgentCommissionDTO2
    {
        public long CommissionId { get; set; }
        public long AgentId { get; set; }
        public long PolicyId { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal CommissionPercentage { get; set; }
        public decimal CommissionAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime EarnedDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
