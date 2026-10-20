
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsuranceProductManagement.Domain.Entities
{
    public class CoverageMaster
    {
        [Key]
        public int coverage_id { get; set; }

        public int product_id { get; set; }

        [StringLength(150)]
        public string ? coverage_name { get; set; }

        public decimal? minimum_coverage { get; set; }

        public decimal? maximum_coverage { get; set; }

        public int? waiting_period_days { get; set; }

        public decimal? deductible_amount { get; set; }
    }
}
