using System;
using System.Collections.Generic;
using System.Text;

namespace ReportsDashboard.Application.DTO
{
    public class UnderwritingDTO
    {
        public long underwriting_id { get; set; }
        public long quote_id { get; set; }
        public long customer_id { get; set; }
        public long product_id { get; set; }
        public DateTime? underwriting_date { get; set; }
        public string? status { get; set; }
        public string? risk_category { get; set; }
        public string? decision { get; set; }
        public long? approved_by { get; set; }
    }
}
