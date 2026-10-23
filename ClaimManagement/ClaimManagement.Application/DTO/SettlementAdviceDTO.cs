using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class SettlementAdviceDTO
    {
        public string advice_number { get; set; } = string.Empty;
        public DateTime generated_on { get; set; }
        public string claim_number { get; set; } = string.Empty;
        public long policy_id { get; set; }
        public long customer_id { get; set; }
        public string claim_type { get; set; } = string.Empty;
        public decimal assessed_amount { get; set; }
        public decimal deductible { get; set; }
        public decimal depreciation { get; set; }
        public decimal non_payable { get; set; }
        public decimal net_payable { get; set; }
        public string payment_mode { get; set; } = string.Empty;
        public string transaction_reference { get; set; } = string.Empty;
        public DateTime settlement_date { get; set; }
        public string settlement_status { get; set; } = string.Empty;
        public string account_holder_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
    }
}
