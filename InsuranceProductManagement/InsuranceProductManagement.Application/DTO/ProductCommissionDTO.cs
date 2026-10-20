using System.ComponentModel.DataAnnotations;

namespace InsuranceProductManagement.Application.DTO
{
    public class ProductCommissionDTO
    {
        public int commission_id { get; set; }

        [Required]
        public int product_id { get; set; }

        public decimal? first_year_commission { get; set; }

        public decimal? renewal_commission { get; set; }

        public decimal? branch_commission { get; set; }
    }
}