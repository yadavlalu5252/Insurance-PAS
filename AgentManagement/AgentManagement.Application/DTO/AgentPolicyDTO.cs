using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.DTO
{
    public class AgentPolicyDTO
    {
        public long PolicyId { get; set; }
        public long CustomerId { get; set; }
        public DateTime? SaleDate { get; set; }
    }
}
