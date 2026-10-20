using System.ComponentModel.DataAnnotations;

namespace InsuranceProductManagement.Application.DTO
{
    public class BenefitMasterDTO
    {
        public int benefit_id { get; set; }

        [Required]
        public int product_id { get; set; }

        [StringLength(150)]
        public string? benefit_name { get; set; }

        public string? benefit_description { get; set; }

        public decimal? benefit_limit { get; set; }
    }
}