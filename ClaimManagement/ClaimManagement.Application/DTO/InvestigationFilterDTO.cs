using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class InvestigationFilterDTO
    {
        public int page_number { get; set; } = 1;
        public int page_size { get; set; } = 10;
        public long? claim_id { get; set; }
        public string? investigation_status { get; set; }
    }
}
