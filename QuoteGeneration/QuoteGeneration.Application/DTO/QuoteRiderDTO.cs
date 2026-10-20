using System.ComponentModel.DataAnnotations;

namespace QuoteGeneration.Application.DTO
{
    public class QuoteRiderDTO
    {
        public int rider_selection_id { get; set; }

        [Required]
        public int quote_id { get; set; }

        [Required]
        public int rider_id { get; set; }

        public decimal? rider_premium { get; set; }
    }
}