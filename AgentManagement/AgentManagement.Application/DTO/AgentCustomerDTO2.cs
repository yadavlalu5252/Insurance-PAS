using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.DTO
{
    public class AgentCustomerDTO2
    {
        public long AgentCustomerId { get; set; }
        public long AgentId { get; set; }
        public long CustomerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public long AssignedBy { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
    }
}
