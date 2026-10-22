using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PolicyServicing.Domain.Entities
{
    public enum renewal_status
    {
        Pending,
        Paid,
        Renewed,
        Expired
    }
    public class policy_renewal
    {
        [Key]
        public long renewal_id { get; set; }

        public long policy_id { get; set; }

        public DateOnly renewal_date { get; set; }

        public DateOnly new_expiry_date { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal premium_amount { get; set; }

        public renewal_status Renewal_Status { get; set; }
    }
}
