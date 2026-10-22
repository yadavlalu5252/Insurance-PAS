using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Underwriting.Domain.Entities
{
    public class underwriting_audit_log
    {
        [Key]
        public int audit_id { get; set; }

        [Required]
        public int underwriting_id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? action { get; set; }

        [Required]
        public int performed_by { get; set; }

        public string? remarks { get; set; }

        [Required]
        public DateTime action_date { get; set; }
    }
}
