using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class PremiumInstallmentDTO
    {
        public long schedule_id { get; set; }
        public long policy_id { get; set; }
        public DateTime due_date { get; set; }
        public string status { get; set; } = string.Empty;
    }
}
