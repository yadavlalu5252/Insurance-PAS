using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class CreateInvestigationDTO
    {
        public long investigator_id { get; set; }
        public string investigation_reason { get; set; } = string.Empty;
    }
}
