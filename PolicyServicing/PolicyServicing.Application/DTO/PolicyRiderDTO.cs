using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class PolicyRiderDTO
    {
        public long policy_rider_id { get; set; }
        public long policy_id { get; set; }
        public long rider_id { get; set; }
        public decimal rider_premium { get; set; }
    }
}
