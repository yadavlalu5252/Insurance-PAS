using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class ClaimFilterDTO
    {
        public int page_number { get; set; } = 1;
        public int page_size { get; set; } = 10;
        public string? search { get; set; }
        public string? claim_status { get; set; }
        public string? claim_type { get; set; }
        public long? customer_id { get; set; }
        public long? policy_id { get; set; }
        public DateTime? from_date { get; set; }
        public DateTime? to_date { get; set; }
    }
}
