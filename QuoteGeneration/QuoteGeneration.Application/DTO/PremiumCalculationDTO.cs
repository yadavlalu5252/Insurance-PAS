using System.ComponentModel.DataAnnotations;

namespace QuoteGeneration.Application.DTO
{
    public class PremiumCalculationDTO
    {
        public int calculation_id { get; set; }

        [Required]
        public int quote_id { get; set; }

        public decimal? base_premium { get; set; }

        public decimal? risk_loading { get; set; }

        public decimal? rider_premium { get; set; }

        public decimal? discount_amount { get; set; }

        public decimal? gst_amount { get; set; }

        public decimal? final_premium { get; set; }
    }
}