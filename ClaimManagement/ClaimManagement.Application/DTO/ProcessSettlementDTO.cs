using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class ProcessSettlementDTO
    {
        public string payment_mode { get; set; } = string.Empty;        // NEFT, RTGS, IMPS, UPI, Cheque
        public string transaction_reference { get; set; } = string.Empty;   // UTR / UPI ref / cheque number
        public string account_holder_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string ifsc_code { get; set; } = string.Empty;

        // Admin confirms the bank account was checked. This is a MANUAL confirmation, not an automatic check.
        public bool bank_verified { get; set; }
    }
}
