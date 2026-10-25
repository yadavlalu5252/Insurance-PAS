
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportsDashboard.Domain.Entities
{
    public class report_schedule
    {
        [Key]
        public long schedule_id { get; set; }

        [Required]
        public long report_id { get; set; }

        [Required]
        [MaxLength(20)]
        public string schedule_type { get; set; } = "";

        [Required]
        public DateTime next_run_date { get; set; }

        [Required]
        [MaxLength(250)]
        public string email_to { get; set; } = "";

        [Required]
        [MaxLength(20)]
        public string status { get; set; } = "Active";

        [ForeignKey("report_id")]
        public report_master? report { get; set; }
    }
}
