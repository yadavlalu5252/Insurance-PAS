
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReportsDashboard.Domain.Entities
{
    public class report_master
    {
        [Key]
        public long report_id { get; set; }

        [Required]
        [MaxLength(150)]
        public string report_name { get; set; } = "";

        [Required]
        [MaxLength(100)]
        public string report_type { get; set; } = "";

        public string? description { get; set; }

        public DateTime created_date { get; set; }

        public List<report_schedule> schedules { get; set; } = new List<report_schedule>();
    }
}
