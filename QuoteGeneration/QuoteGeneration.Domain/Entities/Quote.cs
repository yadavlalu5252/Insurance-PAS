using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuoteGeneration.Domain.Entities
{
    public class Quote
    {
        [Key]
        public int quote_id { get; set; }

        [StringLength(30)]
        public string? quote_number { get; set; }

        public int customer_id { get; set; }

        public int product_id { get; set; }

        [Required]
        public int policy_term { get; set; }

        [Required]
        public decimal sum_insured { get; set; }

        public int? premium_frequency_id { get; set; }

        [Required]
        public DateTime quote_date { get; set; }

        [Required]
        public DateTime expiry_date { get; set; }

        public string quote_status { get; set; } = "Pending";
    }
}
