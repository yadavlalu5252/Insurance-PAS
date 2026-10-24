
using System;
using System.ComponentModel.DataAnnotations;

namespace ReportsDashboard.Application.DTO
{
    public class savescheduledreportdto
    {
        [Required]
        public long report_id { get; set; }

        [Required]
        [RegularExpression("^(Daily|Weekly|Monthly|Quarterly|Yearly)$")]
        public string schedule_type { get; set; } = "";

        [Required]
        public DateTime next_run_date { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(250)]
        public string email_to { get; set; } = "";

        [Required]
        [RegularExpression("^(Active|Inactive)$")]
        public string status { get; set; } = "Active";
    }
}
