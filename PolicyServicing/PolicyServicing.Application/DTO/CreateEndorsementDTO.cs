using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class CreateEndorsementDTO
    {
        public long RequestId { get; set; }
        public string EndorsementType { get; set; } = string.Empty;
        public string PreviousValue { get; set; } = string.Empty;
        public string UpdatedValue { get; set; } = string.Empty;
    }
}
