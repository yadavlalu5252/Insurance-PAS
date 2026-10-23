using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class ClaimSettlement
    {
        [Key]
        public long settlement_id { get; set; }
        public long claim_id { get; set; }
        public decimal approved_amount { get; set; }
        public string payment_mode { get; set; } = string.Empty;
        public string transaction_reference { get; set; } = string.Empty;   // stored ENCRYPTED
        public DateTime settlement_date { get; set; }
        public string settlement_status { get; set; } = "Pending";          // Pending / Paid / Failed (BRD)

        // recommendation: bank details (not in the BRD table)
        public string account_holder_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;          // stored ENCRYPTED
        public string ifsc_code { get; set; } = string.Empty;
        public bool bank_verified { get; set; }                             // manual confirmation by Admin
        public long processed_by { get; set; }
    }
}
