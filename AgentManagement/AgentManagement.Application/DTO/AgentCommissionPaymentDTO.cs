using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.DTO
{
    public class AgentCommissionPaymentDTO
    {
        public long AgentId { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
    }
}
