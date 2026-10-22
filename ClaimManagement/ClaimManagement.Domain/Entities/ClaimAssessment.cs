using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClaimManagement.Domain.Entities
{
    public class ClaimAssessment
    {
        [Key]
        public long assessment_id { get; set; }
        public long claim_id { get; set; }
        public decimal assessed_amount { get; set; }
        public decimal deductible { get; set; }
        public decimal depreciation { get; set; }
        public decimal non_payable { get; set; }
        public decimal net_payable { get; set; }
        public long assessed_by { get; set; }        
        public DateTime assessed_date { get; set; } 
    }
}
