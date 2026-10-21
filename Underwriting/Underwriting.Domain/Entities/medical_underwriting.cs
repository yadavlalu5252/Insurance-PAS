using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Underwriting.Domain.Entities
{
    public class medical_underwriting
    {
        [Key]
        public long medical_id { get; set; }

        [Required]
        public long underwriting_id { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal height { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal weight { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal bmi { get; set; }

        [Required]
        public Boolean smoking { get; set; }

        [Required]
        public Boolean alcohol { get; set; }

        [Required]
        public string medical_history { get; set; }

        [Required]
        public string medical_report_path { get; set; }
    }
}
