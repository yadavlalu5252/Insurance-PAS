using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Underwriting.Domain.Entities
{
    public class financial_underwriting
    {
        [Key]
        public long financial_id { get; set; }

        [Required]
        public long underwriting_id { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal annual_income { get; set; }

        [Required]
        public string employer_name { get; set; }

        [Required]
        public string occupation { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal existing_insurance { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal loan_liability { get; set; }
    }
}
