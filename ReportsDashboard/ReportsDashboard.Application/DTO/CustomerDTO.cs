using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class CustomerDTO
    {
        public long customer_id { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? kyc_status { get; set; }
        public string? status { get; set; }
        public long? agent_id { get; set; }
        public DateTime? registration_date { get; set; }
    }
}
