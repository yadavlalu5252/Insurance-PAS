using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class NomineeUpdateDTO
    {
        // We send a list because the sum of all nominees must equal 100%
        public List<NomineeDTO> Nominees { get; set; } = new();
    }
}
