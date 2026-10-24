
using System;
using System.ComponentModel.DataAnnotations;

namespace AgentManagement.Domain.Entities
{
    public class AgentPolicy
    {
        [Key]
        public long AgentPolicyId { get; set; }

        public long AgentId { get; set; }
        public long PolicyId { get; set; }
        public long CustomerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? SaleDate { get; set; }
        public string Status { get; set; } = "Assigned";
        public DateTime CreatedAt { get; set; }
    }
}
