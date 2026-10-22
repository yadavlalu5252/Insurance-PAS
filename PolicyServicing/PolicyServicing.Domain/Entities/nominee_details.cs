using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PolicyServicing.Domain.Entities
{
    public class nominee_details
    {
        [Key]
        public long nominee_id { get; set; }

        public long policy_id { get; set; }

        public string nominee_name { get; set; }

        public string relationship { get; set; } = string.Empty;

        public DateOnly dob { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal share_percentage { get; set; }

        public string guardian_name { get; set; }
    }
}
