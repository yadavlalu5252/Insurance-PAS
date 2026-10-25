
using System;
using System.ComponentModel.DataAnnotations;

namespace AgentManagement.Domain.Entities
{
    public class AgentCustomer
    {
        [Key]
        public long AgentCustomerId { get; set; }

        public long AgentId { get; set; }
        public long CustomerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public long AssignedBy { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
    }
}
