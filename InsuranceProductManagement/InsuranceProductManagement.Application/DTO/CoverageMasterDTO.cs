
using System.ComponentModel.DataAnnotations;

namespace InsuranceProductManagement.Application.DTO
{
    public class CoverageMasterDTO
    {
        public int coverage_id { get; set; }

        [Required]
        public int product_id { get; set; }

        [StringLength(150)]
        public string? coverage_name { get; set; }

        public decimal? minimum_coverage { get; set; }

        public decimal? maximum_coverage { get; set; }

        public int? waiting_period_days { get; set; }

        public decimal? deductible_amount { get; set; }
    }
}
