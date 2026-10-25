
using System;

namespace ReportsDashboard.Application.DTO
{
    public class scheduledreportdto
    {
        public long schedule_id { get; set; }

        public long report_id { get; set; }

        public string? report_name { get; set; }

        public string schedule_type { get; set; } = "";

        public DateTime next_run_date { get; set; }

        public string email_to { get; set; } = "";

        public string status { get; set; } = "";
    }
}
