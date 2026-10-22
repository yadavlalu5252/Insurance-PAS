using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Underwriting.Domain.Entities
{
    public class underwriting_decision
    {
        [Key]
        public int decision_id { get; set; }

        [Required]
        public int underwriting_id { get; set; }

        [Required]
        public string  decision { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal loading_percentage { get; set; }

        
        public string? exclusion_details { get; set; }

        
        public string? rejection_reason { get; set; }

        [Required]
        public int approved_by { get; set; }

        [Required]
        public DateTime decision_date { get; set; }

    }
}
