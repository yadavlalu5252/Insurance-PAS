using System.ComponentModel.DataAnnotations;

namespace QuoteGeneration.Application.DTO
{
    public class QuoteHistoryDTO
    {
        public int history_id { get; set; }

        [Required]
        public int quote_id { get; set; }

        [StringLength(100)]
        public string? action { get; set; }

        public int? performed_by { get; set; }

        public DateTime? action_date { get; set; }

        [StringLength(500)]
        public string? remarks { get; set; }
    }
}