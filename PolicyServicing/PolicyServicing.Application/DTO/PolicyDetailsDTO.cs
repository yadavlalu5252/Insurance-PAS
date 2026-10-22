using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class PolicyDetailsDTO : PolicyDTO
    {
        public List<PolicyCoverageDTO> coverages { get; set; } = new();

        public List<PolicyRiderDTO> riders { get; set; } = new();

    }
}
