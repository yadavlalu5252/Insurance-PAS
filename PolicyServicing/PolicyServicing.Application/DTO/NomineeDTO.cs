using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class NomineeDTO
    {
        public long nominee_id { get; set; } // 0 if adding new

        public long customer_id { get; set; }
        public string nominee_name { get; set; }
        public string relationship { get; set; }
        public DateTime date_of_birth { get; set; }
        public string mobile_number { get; set; }
        public decimal share_percentage { get; set; }

        // Required for Core Policy Microservice (Minor check)
        public string? guardian_name { get; set; }
    }
}
