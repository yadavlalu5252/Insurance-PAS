using System;
using System.Collections.Generic;
using System.Text;

namespace Underwriting.Application.DTO
{
    public class savefinancialdto
    {
        public decimal annual_income { get; set; }
        public string employer_name { get; set; }
        public string occupation { get; set; }
        public decimal existing_insurance { get; set; }
        public decimal loan_liability { get; set; }
    }
}
