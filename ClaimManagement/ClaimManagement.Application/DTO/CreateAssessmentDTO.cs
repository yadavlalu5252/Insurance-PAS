using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class CreateAssessmentDTO
    {
        public decimal assessed_amount { get; set; }
        public decimal deductible { get; set; }
        public decimal depreciation { get; set; }
        public decimal non_payable { get; set; }
    }
}
