using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PolicyServicing.Domain.Entities
{
    public enum RequestStatus
    {
        Submitted,
        UnderReview,
        Approved,
        Rejected,
        Completed,
        Closed
    }
    public class service_request
    {
        [Key]
        public long request_id { get; set; }

        public string request_number { get; set; } = string.Empty;

        public long policy_id { get; set; }

        public long customer_id { get; set; }

        public string request_type { get; set; } = string.Empty;

        public DateTime request_date { get; set; }

        public RequestStatus status { get; set; }

        public List<policy_endorsement> policy_endorsements { get; set; } = new List<policy_endorsement>();

    }
}
