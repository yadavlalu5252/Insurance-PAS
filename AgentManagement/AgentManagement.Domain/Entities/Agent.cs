
using System;
using System.ComponentModel.DataAnnotations;

namespace AgentManagement.Domain.Entities
{
    public class Agent
    {
        [Key]
        public long AgentId { get; set; }

        public long UserId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public string? LicenseNumber { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
