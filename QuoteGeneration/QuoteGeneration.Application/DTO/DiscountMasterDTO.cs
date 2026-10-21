using System.ComponentModel.DataAnnotations;

namespace QuoteGeneration.Application.DTO
{
    public class DiscountMasterDTO
    {
        public int discount_id { get; set; }

        [Required]
        [StringLength(100)]
        public string? discount_name { get; set; }

        public decimal? discount_percentage { get; set; }

        public DateTime? valid_from { get; set; }

        public DateTime? valid_to { get; set; }

        public string? status { get; set; } = "Active";
    }
}