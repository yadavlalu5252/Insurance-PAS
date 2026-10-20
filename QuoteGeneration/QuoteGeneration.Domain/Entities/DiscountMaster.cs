using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuoteGeneration.Domain.Entities
{
    public class DiscountMaster
    {
        [Key]
        public int discount_id { get; set; }

        [StringLength(100)]
        public string? discount_name { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? discount_percentage { get; set; }

        public DateTime? valid_from { get; set; }

        public DateTime? valid_to { get; set; }

        public string? status { get; set; }
    }
}
