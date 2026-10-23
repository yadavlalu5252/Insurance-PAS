using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PolicyServicing.Domain.Entities
{
    public class PolicyCancellation
    {
        [Key]
        public long cancellation_id { get; set; }

        public long policy_id { get; set; }

        public DateOnly cancellation_date { get; set; }

        public string cancellation_reason { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal refund_amount { get; set; }

        public long approved_by { get; set; }
    }
}
