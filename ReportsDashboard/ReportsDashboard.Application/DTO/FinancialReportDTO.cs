using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class FinancialReportDTO
    {
        public decimal premium_income { get; set; }
        public decimal claim_payments { get; set; }
        public decimal outstanding_premium { get; set; }
        public decimal refunds { get; set; }
        public decimal branch_revenue { get; set; }
        public decimal monthly_profit { get; set; }
    }
}
