using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Underwriting.Domain.Entities
{
    public class risk_score
    {
        [Key]
        public int risk_id { get; set; }

        [Required]
        public int underwriting_id { get; set; }
        [Required]
        public int age_score { get; set; }

        [Required]
        public int medical_score { get; set; }

        [Required]
        public int occupation_score { get; set; }

        [Required]
        public int lifestyle_score { get; set; }

        [Required]
        public int financial_score { get; set; }

        [Required]
        public int total_score { get; set; }

        [Required]
        public string risk_category { get; set; }
    }
}
