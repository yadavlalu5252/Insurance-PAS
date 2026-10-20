
using System.ComponentModel.DataAnnotations;

namespace InsuranceProductManagement.Application.DTO
{
    public class InsuranceProductDTO
    {
        public int product_id { get; set; }

        [Required]
        public int category_id { get; set; }

        [Required]
        [StringLength(30)]
        public string ?  product_code { get; set; } 

        [Required]
        [StringLength(150)]
        public string ?  product_name { get; set; }

        [Required]
        [StringLength(100)]
        public string ?  product_type { get; set; } 

        public string? description { get; set; }

        public int? minimum_age { get; set; }

        public int? maximum_age { get; set; }

        public decimal? minimum_sum_insured { get; set; }

        public decimal? maximum_sum_insured { get; set; }

        public string status { get; set; } = "Active";
    }
}
