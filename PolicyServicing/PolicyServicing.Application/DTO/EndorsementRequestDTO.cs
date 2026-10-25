using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PolicyServicing.Application.DTO
{
    public class EndorsementRequestDTO
    {
        public long PolicyId { get; set; }

        [JsonIgnore]
        public long CustomerId { get; set; }

        public string EndorsementType { get; set; }

        public string PayloadJson { get; set; }
    }
}
