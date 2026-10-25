using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class PolicyRiderDTO
    {
        public long rider_id { get; set; }
        public long policy_id { get; set; }
        public string rider_name { get; set; } = string.Empty; 
        public decimal premium_amount { get; set; } 
    }
}