using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class SettlementResponseDTO
    {
        public long settlement_id { get; set; }
        public long claim_id { get; set; }
        public decimal approved_amount { get; set; }
        public string payment_mode { get; set; } = string.Empty;
        public string transaction_reference { get; set; } = string.Empty;
        public DateTime settlement_date { get; set; }
        public string settlement_status { get; set; } = string.Empty;
        public string account_holder_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;
        public bool bank_verified { get; set; }
    }
}
