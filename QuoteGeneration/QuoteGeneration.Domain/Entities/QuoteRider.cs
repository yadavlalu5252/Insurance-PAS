using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QuoteGeneration.Domain.Entities
{
    public class QuoteRider
    {
        [Key]
        public int rider_selection_id { get; set; }

        public int quote_id { get; set; }

        public int rider_id { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal? rider_premium { get; set; }
    }
}
