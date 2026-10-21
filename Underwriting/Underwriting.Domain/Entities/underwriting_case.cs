using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Underwriting.Domain.Entities
{
    public class underwriting_case
    {

        [Key]
        public long underwriting_id { get; set; }

        [Required]
        public long quote_id { get; set; }

        [Required]
        public long customer_id { get; set; }

        [Required]
        public long product_id { get; set; }

        [Required]
        public DateTime underwriting_date { get; set; }

        [Required]
        public string status { get; set; } = "Pending";
    }

}

