using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PolicyServicing.Domain.Entities
{
    public enum Approval_status
    {
        Pending,
        Approved,
        Rejected
    }
    public class policy_endorsement
    {
        [Key]
        public long endorsement_id { get; set; }

        [ForeignKey("request_id")]

        public long request_id { get; set; }

        public service_request service_Request { get; set; }

        public string endorsement_number { get; set; } = string.Empty;

        public string endorsement_type { get; set; } = string.Empty;

        public string previous_value { get; set; } = string.Empty;

        public string updated_value { get; set; } = string.Empty;

        public Approval_status approval_status { get; set; }


    }
}
