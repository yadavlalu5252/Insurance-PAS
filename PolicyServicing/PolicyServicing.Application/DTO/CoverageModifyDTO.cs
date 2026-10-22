using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class CoverageModifyDTO
    {
        public long CoverageId { get; set; }
        public decimal NewSumInsured { get; set; }
    }
}
