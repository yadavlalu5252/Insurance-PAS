using System.ComponentModel.DataAnnotations;

namespace InsuranceProductManagement.Application.DTO
{
    public class RiderMasterDTO
    {
        public int rider_id { get; set; }

        [Required]
        public int product_id { get; set; }

        [StringLength(150)]
        public string? rider_name { get; set; }

        public string? rider_description { get; set; }

        public decimal? rider_premium { get; set; }

        public string? status { get; set; } = "Active";
    }
}