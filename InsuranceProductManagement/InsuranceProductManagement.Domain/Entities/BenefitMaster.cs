
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceProductManagement.Domain.Entities
{
    public class BenefitMaster
    {
        [Key]
        public int benefit_id { get; set; }

        public int product_id { get; set; }

        [StringLength(150)]
        public string ? benefit_name { get; set; } 

        public string ? benefit_description { get; set; }

        public decimal? benefit_limit { get; set; }
    }
}
