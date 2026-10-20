
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceProductManagement.Domain.Entities
{
    public class PremiumFrequency
    {
        [Key]
        public int frequency_id { get; set; }

        [Required]
        [RegularExpression("^(Monthly|Quarterly|Half-Yearly|Yearly|Single Premium)$")]
        public string ? frequency_name { get; set; }

        public int? installment_count { get; set; }

        public string status { get; set; } 
    }
}
