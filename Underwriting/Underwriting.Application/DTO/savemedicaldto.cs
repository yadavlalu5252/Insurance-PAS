using System;
using System.Collections.Generic;
using System.Text;

namespace Underwriting.Application.DTO
{
    public class savemedicaldto
    {
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public decimal bmi { get; set; }
        public bool smoking { get; set; }
        public bool alcohol { get; set; }
        public string medical_history { get; set; }
        public string medical_report_path { get; set; }
    }
}
