using System.ComponentModel.DataAnnotations;

namespace QuoteGeneration.Application.DTO
{
    public class QuoteDTO
    {
        public int quote_id { get; set; }

        public string? quote_number { get; set; }

        [Required]
        public int customer_id { get; set; }

        [Required]
        public int product_id { get; set; }

        [Required]
        public int policy_term { get; set; }

        [Required]
        public decimal sum_insured { get; set; }

        public int? premium_frequency_id { get; set; }

        public DateTime quote_date { get; set; }

        public DateTime expiry_date { get; set; }

        public string? quote_status { get; set; } = "Pending";
    }
}