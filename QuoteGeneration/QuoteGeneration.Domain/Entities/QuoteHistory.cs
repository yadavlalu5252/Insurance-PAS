using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuoteGeneration.Domain.Entities
{
    public class QuoteHistory
    {
        [Key]
        public int history_id { get; set; }

        public int quote_id { get; set; }

        [StringLength(100)]
        public string? action { get; set; }

        public int? performed_by { get; set; }

        public DateTime? action_date { get; set; }

        [StringLength(500)]
        public string? remarks { get; set; }
    }
}
