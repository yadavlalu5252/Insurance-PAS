using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class AgentReportDTO
    {
        public long agent_id { get; set; }
        public string? agent_name { get; set; }
        public int total_sales { get; set; }
        public decimal premium_collected { get; set; }
        public int active_customers { get; set; }
        public decimal commission { get; set; }
        public decimal claim_ratio { get; set; }
        public string? report_month { get; set; }
    }
}
