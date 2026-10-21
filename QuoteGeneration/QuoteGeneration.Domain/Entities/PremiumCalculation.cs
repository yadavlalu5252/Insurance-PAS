using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuoteGeneration.Domain.Entities
{
    public class PremiumCalculation
    {
        [Key]
        public int calculation_id { get; set; }

        public int quote_id { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? base_premium { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? risk_loading { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? rider_premium { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? discount_amount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? gst_amount { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? final_premium { get; set; }
    }
}
