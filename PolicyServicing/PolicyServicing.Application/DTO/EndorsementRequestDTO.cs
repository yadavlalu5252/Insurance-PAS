using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    public class EndorsementRequestDTO
    {
        public long PolicyId { get; set; }

        public long CustomerId { get; set; }

        public string EndorsementType { get; set; }

        public string PayloadJson { get; set; }
    }
}
