using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class CancellationRequestDTO
    {
        public string CancellationReason { get; set; }

        public long ApprovedBy { get; set; }
    }
}
